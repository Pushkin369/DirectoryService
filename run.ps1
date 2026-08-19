# ============================================================
#  DirectoryService - панель управления: Docker + EF Core + psql
#
#  Меню (двойной клик по run.cmd или):
#      powershell -NoProfile -ExecutionPolicy Bypass -File run.ps1
#
#  Разовые действия из терминала (без меню):
#      .\run.ps1 -Action start                  # поднять контейнеры
#      .\run.ps1 -Action mig-add -Arg Init      # создать миграцию
#      .\run.ps1 -Action mig-update             # накатить миграции (создаёт БД)
#      .\run.ps1 -Action tables                 # psql \dt
#      .\run.ps1 -Action db-drop -Yes           # подтвердить опасное действие
#      .\run.ps1 -Action help                   # список всех действий
# ============================================================

param(
    [string]$Action,   # разовое действие (см. help); без него откроется меню
    [string]$Arg,      # аргумент действия: имя миграции, имя таблицы, целевая миграция
    [switch]$Yes       # подтверждение опасных действий при запуске из терминала
)

# Кодировка UTF-8 для вывода (чтобы кириллица не превращалась в кракозябры)
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8

# Переходим в папку, где лежит этот скрипт (рядом с docker-compose.yml),
# чтобы docker compose находил yml-файл откуда бы ни запустили
Set-Location -Path $PSScriptRoot

# ---------- Пути и константы ----------

# Проект с AppDbContext и миграциями (куда кладёт dotnet ef migrations add)
$EfProject = Join-Path $PSScriptRoot "backend\DirectoryService\src\DirectoryService.Infrastructure.Postgres"
# Startup-проект для dotnet ef: источник строки подключения и DI
$EfStartup = Join-Path $PSScriptRoot "backend\DirectoryService\src\DirectoryService.Web"
# Имя контейнера Postgres из docker-compose.yml (container_name)
$DbContainer = "directoryservice-postgres"

# ---------- Чтение .env (нужны имя пользователя и базы для psql) ----------

function Get-EnvValue {
    param([string]$Key)
    $envFile = Join-Path $PSScriptRoot ".env"
    if (-not (Test-Path $envFile)) { return $null }
    foreach ($line in Get-Content $envFile) {
        if ($line -match "^\s*$([regex]::Escape($Key))=(.*)$") {
            return $Matches[1].Trim()
        }
    }
    return $null
}

$DbUser = Get-EnvValue "POSTGRES_USER"
if (-not $DbUser) { $DbUser = "postgres" }
$DbName = Get-EnvValue "POSTGRES_DB"
if (-not $DbName) { $DbName = "postgres" }

# ---------- Функции-помощники ----------

# Печать команды перед выполнением - чтобы запоминать, что под каждой кнопкой
function Invoke-Step {
    param([string]$Label, [scriptblock]$Cmd)
    Write-Host ""
    Write-Host ">> $Label" -ForegroundColor DarkGray
    & $Cmd
}

function Wait-Enter {
    Write-Host ""
    Read-Host "Нажми Enter, чтобы вернуться в меню" | Out-Null
}

# Подтверждение опасного действия: YES в меню, флаг -Yes из терминала
function Confirm-Danger {
    param([string]$Message)
    if ($Yes) { return $true }
    Write-Host ""
    Write-Host "   ВНИМАНИЕ: $Message" -ForegroundColor Red
    $confirm = Read-Host "   Введи YES (заглавными) для подтверждения"
    # -ceq = сравнение с учётом регистра: сработает только на точное YES
    return ($confirm -ceq "YES")
}

# Установлен ли глобальный инструмент dotnet-ef
function Test-EfTool {
    dotnet ef --version *> $null
    return ($LASTEXITCODE -eq 0)
}

# Проверка перед действиями, которым нужен живой Postgres
function Test-PostgresRunning {
    $state = docker inspect -f "{{.State.Running}}" $DbContainer 2>$null | Out-String
    return ($state.Trim() -eq "true")
}

function Require-Postgres {
    if (-not (Test-PostgresRunning)) {
        Write-Host ""
        Write-Host "   Контейнер '$DbContainer' не запущен." -ForegroundColor Yellow
        Write-Host "   Сначала запусти инфраструктуру: пункт 1 (START) или .\run.ps1 -Action start" -ForegroundColor Yellow
        return $false
    }
    return $true
}

# Обёртка над dotnet ef: подставляет --project/--startup-project везде
function Invoke-Ef {
    param([Parameter(Mandatory = $true, ValueFromRemainingArguments = $true)][string[]]$EfArgs)
    & dotnet ef @EfArgs --project $EfProject --startup-project $EfStartup
}

# ---------- Действия: Docker ----------

function Invoke-Start {
    Invoke-Step "docker compose up -d" { docker compose up -d }
    Invoke-Step "docker compose ps"    { docker compose ps }
}

function Invoke-Status {
    Invoke-Step "docker compose ps" { docker compose ps }
}

function Invoke-Logs {
    Invoke-Step "docker compose logs -f postgres" { docker compose logs -f postgres }
    # после Ctrl+C скрипт завершится целиком - это нормально
}

function Invoke-Stop {
    Invoke-Step "docker compose down" { docker compose down }
}

function Invoke-Reset {
    if (-not (Confirm-Danger "будут удалены ВСЕ данные базы данных!")) {
        Write-Host "   Отменено." -ForegroundColor Yellow
        return
    }
    Invoke-Step "docker compose down -v" { docker compose down -v }
    Write-Host ""
    Write-Host "   Данные удалены. База создастся заново при следующем START (1)"
    Write-Host "   + накатывании миграций (8 - UPDATE DATABASE)."
}

# ---------- Действия: EF Core миграции ----------

function Invoke-MigList {
    Invoke-Step "dotnet ef migrations list" { Invoke-Ef migrations list }
}

function Invoke-MigAdd {
    param([string]$Name)
    if ([string]::IsNullOrWhiteSpace($Name)) {
        if ($Action) { Write-Host "   Укажи имя: .\run.ps1 -Action mig-add -Arg <ИмяМиграции>" -ForegroundColor Yellow; return }
        $Name = Read-Host "Имя миграции (PascalCase, например AddUserTable)"
    }
    if ($Name -notmatch "^[A-Za-z_][A-Za-z0-9_]*$") {
        Write-Host "   Недопустимое имя '$Name': только латиница, цифры и _ , без пробелов" -ForegroundColor Yellow
        return
    }
    Invoke-Step "dotnet ef migrations add $Name" { Invoke-Ef migrations add $Name }
    Write-Host ""
    Write-Host "   Миграция создана в $EfProject\Migrations." -ForegroundColor Green
    Write-Host "   Накатить: пункт 8 (UPDATE DATABASE) или -Action mig-update."
}

function Invoke-MigUpdate {
    param([string]$Target)
    # Пустой target = накатить всё до последней; БД создаётся автоматически, если её нет
    if ([string]::IsNullOrWhiteSpace($Target)) {
        Invoke-Step "dotnet ef database update (до последней)" { Invoke-Ef database update }
    }
    else {
        Invoke-Step "dotnet ef database update $Target" { Invoke-Ef database update $Target }
    }
}

function Invoke-MigScript {
    # Idempotent-скрипт содержит всю историю и проверки "__EFMigrationsHistory":
    # его можно безопасно выполнять на любой базе (в т.ч. пустой и уже накатанной)
    $dir = Join-Path $PSScriptRoot "artifacts\migrations"
    New-Item -ItemType Directory -Force -Path $dir | Out-Null
    $path = Join-Path $dir ("script_" + (Get-Date -Format "yyyyMMdd_HHmmss") + ".sql")
    Invoke-Step "dotnet ef migrations script --idempotent" { Invoke-Ef migrations script --idempotent --output $path }
    if (Test-Path $path) {
        Write-Host ""
        Write-Host "   Скрипт сохранён: $path" -ForegroundColor Green
        Write-Host "   Применить руками: docker exec -i $DbContainer psql -U $DbUser -d $DbName < `"$path`""
    }
}

function Invoke-MigRemove {
    Write-Host ""
    Write-Host "   Удаляется ПОСЛЕДНЯЯ миграция, и только если она ещё НЕ применена к базе." -ForegroundColor Yellow
    Write-Host "   Если уже применена - сначала откатись на предыдущую: -Action mig-update -Arg <Предыдущая>," -ForegroundColor Yellow
    Write-Host "   затем запускай удаление." -ForegroundColor Yellow
    Invoke-Step "dotnet ef migrations remove" { Invoke-Ef migrations remove }
}

function Invoke-DbInfo {
    # Провайдер, строка подключения и прочее о AppDbContext
    Invoke-Step "dotnet ef dbcontext info" { Invoke-Ef dbcontext info }
}

# ---------- Действия: база данных ----------

function Invoke-DbDrop {
    if (-not (Confirm-Danger "будет удалена база '$DbName' (контейнер и данные тома останутся)!")) {
        Write-Host "   Отменено." -ForegroundColor Yellow
        return
    }
    Invoke-Step "dotnet ef database drop --force" { Invoke-Ef database drop --force }
}

function Invoke-DbRecreate {
    if (-not (Confirm-Danger "база '$DbName' будет удалена и создана заново со всеми миграциями!")) {
        Write-Host "   Отменено." -ForegroundColor Yellow
        return
    }
    Invoke-Step "dotnet ef database drop --force" { Invoke-Ef database drop --force }
    Invoke-Step "dotnet ef database update"       { Invoke-Ef database update }
}

function Invoke-Tables {
    if (-not (Require-Postgres)) { return }
    Invoke-Step "psql \dt" { docker exec $DbContainer psql -U $DbUser -d $DbName -c "\dt" }
}

function Invoke-Describe {
    param([string]$Table)
    if ([string]::IsNullOrWhiteSpace($Table)) {
        if ($Action) { Write-Host "   Укажи таблицу: .\run.ps1 -Action describe -Arg departments" -ForegroundColor Yellow; return }
        $Table = Read-Host "Имя таблицы (например departments)"
    }
    if (-not (Require-Postgres)) { return }
    Invoke-Step "psql \d $Table" { docker exec $DbContainer psql -U $DbUser -d $DbName -c "\d $Table" }
}

function Invoke-PsqlShell {
    if (-not (Require-Postgres)) { return }
    Write-Host ""
    Write-Host ">> psql: база $DbName, пользователь $DbUser (выход: \q)" -ForegroundColor DarkGray
    docker exec -it $DbContainer psql -U $DbUser -d $DbName
}

# ---------- Справка и диспетчер разовых действий ----------

function Show-Help {
    Write-Host ""
    Write-Host "Разовые действия: .\run.ps1 -Action <имя> [-Arg <аргумент>] [-Yes]" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "  Docker:"
    Write-Host "    start        - запустить контейнеры"
    Write-Host "    status       - статус контейнеров"
    Write-Host "    logs         - логи Postgres (выход Ctrl+C)"
    Write-Host "    stop         - остановить (данные сохраняются)"
    Write-Host "    reset [-Yes] - остановить и удалить ВСЕ данные БД"
    Write-Host "  EF Core:"
    Write-Host "    mig-list     - список миграций"
    Write-Host "    mig-add      - создать миграцию: -Arg <ИмяМиграции>"
    Write-Host "    mig-update   - накатить миграции: -Arg <Целевая|пусто=все>; БД создаётся сама"
    Write-Host "    mig-script   - idempotent SQL-скрипт всех миграций в artifacts\migrations"
    Write-Host "    mig-remove   - удалить последнюю НЕприменённую миграцию"
    Write-Host "    db-info      - информация о AppDbContext и подключении"
    Write-Host "  База (psql):"
    Write-Host "    db-drop      [-Yes] - удалить базу '$DbName'"
    Write-Host "    db-recreate  [-Yes] - удалить базу и накатить миграции заново"
    Write-Host "    tables       - список таблиц (\dt)"
    Write-Host "    describe     - структура таблицы (\d): -Arg <таблица>"
    Write-Host "    psql         - интерактивная консоль psql"
    Write-Host ""
}

function Invoke-Action {
    param([string]$Name)
    if ([string]::IsNullOrWhiteSpace($Name)) { return }
    if (-not (Test-EfTool)) {
        Write-Host ""
        Write-Host "   dotnet-ef не найден. Установи (один раз):" -ForegroundColor Yellow
        Write-Host "   dotnet tool install --global dotnet-ef --ignore-failed-sources" -ForegroundColor Yellow
        Write-Host ""
        return
    }
    switch ($Name.ToLowerInvariant()) {
        "start"       { Invoke-Start }
        "status"      { Invoke-Status }
        "logs"        { Invoke-Logs }
        "stop"        { Invoke-Stop }
        "reset"       { Invoke-Reset }
        "mig-list"    { Invoke-MigList }
        "mig-add"     { Invoke-MigAdd -Name $Arg }
        "mig-update"  { Invoke-MigUpdate -Target $Arg }
        "mig-script"  { Invoke-MigScript }
        "mig-remove"  { Invoke-MigRemove }
        "db-info"     { Invoke-DbInfo }
        "db-drop"     { Invoke-DbDrop }
        "db-recreate" { Invoke-DbRecreate }
        "tables"      { Invoke-Tables }
        "describe"    { Invoke-Describe -Table $Arg }
        "psql"        { Invoke-PsqlShell }
        "help"        { Show-Help }
        default {
            Write-Host "Неизвестное действие '$Name'." -ForegroundColor Yellow
            Show-Help
        }
    }
}

# ---------- Точка входа ----------

# Разовый запуск из терминала: .\run.ps1 -Action <имя> ...
# Важно: без [void]/присваивания - иначе вывод docker/dotnet из функций
# (output pipeline) будет выброшен вместо печати в консоль
if ($Action) {
    Invoke-Action -Name $Action
    exit 0
}

# ---------- Меню (двухуровневое: раздел -> действия) ----------

function Clear-Screen {
    # try/catch: Clear-Host падает, если скрипт запущен без настоящей консоли
    # (например, с перенаправленным выводом) - меню просто выведется поверх
    try { Clear-Host } catch { }
}

function Show-Header {
    param([string]$Title)
    Clear-Screen
    Write-Host "==================================================" -ForegroundColor Cyan
    Write-Host "          DirectoryService - панель управления"      -ForegroundColor Cyan
    Write-Host "==================================================" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "  --- $Title ---" -ForegroundColor White
    Write-Host ""
}

# Подменю Docker
function Show-DockerMenu {
    do {
        Show-Header "Docker (инфраструктура)"
        Write-Host "   1. START   - запустить контейнеры"
        Write-Host "   2. STATUS  - статус контейнеров"
        Write-Host "   3. LOGS    - логи Postgres (выход: Ctrl+C)"
        Write-Host "   4. STOP    - остановить (данные сохраняются)"
        Write-Host "   5. RESET   - остановить И удалить все данные БД" -ForegroundColor Red
        Write-Host ""
        Write-Host "   0. НАЗАД   - в главное меню"
        Write-Host ""
        $choice = Read-Host "Выбери пункт"

        # do/until вместо break: break внутри switch прервал бы switch, а не цикл
        switch ($choice) {
            "1" { Invoke-Start;  Wait-Enter }
            "2" { Invoke-Status; Wait-Enter }
            "3" { Invoke-Logs }
            "4" { Invoke-Stop;  Wait-Enter }
            "5" { Invoke-Reset; Wait-Enter }
        }
    } until ($choice -eq "0")
}

# Подменю EF Core
function Show-EfMenu {
    do {
        Show-Header "EF Core (миграции)"
        Write-Host "   1. MIG LIST     - список миграций"
        Write-Host "   2. MIG ADD      - создать миграцию"
        Write-Host "   3. UPDATE DB    - накатить миграции (создаёт БД, если нет)"
        Write-Host "   4. SCRIPT       - idempotent SQL-скрипт миграций"
        Write-Host "   5. MIG REMOVE   - удалить последнюю НЕприменённую миграцию"
        Write-Host "   6. DB INFO      - контекст и подключение (dotnet ef dbcontext info)"
        Write-Host ""
        Write-Host "   0. НАЗАД        - в главное меню"
        Write-Host ""
        $choice = Read-Host "Выбери пункт"

        switch ($choice) {
            "1" { Invoke-MigList;   Wait-Enter }
            "2" { Invoke-MigAdd;    Wait-Enter }
            "3" { Invoke-MigUpdate; Wait-Enter }
            "4" { Invoke-MigScript; Wait-Enter }
            "5" { Invoke-MigRemove; Wait-Enter }
            "6" { Invoke-DbInfo;    Wait-Enter }
        }
    } until ($choice -eq "0")
}

# Подменю PostgreSQL
function Show-PsqlMenu {
    do {
        Show-Header "PostgreSQL (psql)"
        Write-Host "   1. TABLES      - список таблиц (\dt)"
        Write-Host "   2. DESCRIBE    - структура таблицы (\d <имя>)"
        Write-Host "   3. PSQL        - интерактивная консоль psql"
        Write-Host "   4. DB DROP     - удалить базу (миграциями накатится заново)" -ForegroundColor Red
        Write-Host "   5. DB RECREATE - удалить базу И накатить миграции заново" -ForegroundColor Red
        Write-Host ""
        Write-Host "   0. НАЗАД       - в главное меню"
        Write-Host ""
        $choice = Read-Host "Выбери пункт"

        switch ($choice) {
            "1" { Invoke-Tables;     Wait-Enter }
            "2" { Invoke-Describe;   Wait-Enter }
            "3" { Invoke-PsqlShell;  Wait-Enter }
            "4" { Invoke-DbDrop;     Wait-Enter }
            "5" { Invoke-DbRecreate; Wait-Enter }
        }
    } until ($choice -eq "0")
}

# Главное меню: выбор раздела
do {
    Clear-Screen
    Write-Host "==================================================" -ForegroundColor Cyan
    Write-Host "          DirectoryService - панель управления"      -ForegroundColor Cyan
    Write-Host "==================================================" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "   1. Docker (инфраструктура)"
    Write-Host "   2. EF Core (миграции)"
    Write-Host "   3. PostgreSQL (psql)"
    Write-Host ""
    Write-Host "   0. EXIT - закрыть"
    Write-Host ""
    Write-Host "   pgAdmin  : http://localhost:5050  (логин/пароль — см. .env.example)"
    Write-Host "   Postgres : localhost:5433         (логин/пароль — см. .env.example)"
    Write-Host ""
    $choice = Read-Host "Выбери раздел"

    switch ($choice) {
        "1" { Show-DockerMenu }
        "2" { Show-EfMenu }
        "3" { Show-PsqlMenu }
    }
} until ($choice -eq "0")

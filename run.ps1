# ============================================================
#  DirectoryService - меню управления Docker (PowerShell)
#
#  Запуск из терминала:
#      powershell -NoProfile -ExecutionPolicy Bypass -File run.ps1
#  или просто двойной клик по run.cmd (он вызывает этот скрипт)
# ============================================================

# Кодировка UTF-8 для вывода (чтобы кириллица не превращалась в кракозябры)
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8

# Переходим в папку, где лежит этот скрипт (рядом с docker-compose.yml),
# чтобы docker compose находил yml-файл откуда бы ни запустили
Set-Location -Path $PSScriptRoot

# ---------- Функции-помощники ----------

function Show-Menu {
    # try/catch: Clear-Host падает, если скрипт запущен без настоящей консоли
    # (например, с перенаправленным выводом) - меню просто выведется поверх
    try { Clear-Host } catch { }
    Write-Host "==================================================" -ForegroundColor Cyan
    Write-Host "          DirectoryService - Docker"               -ForegroundColor Cyan
    Write-Host "==================================================" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "   1. START   - запустить контейнеры"
    Write-Host "   2. STATUS  - статус контейнеров"
    Write-Host "   3. LOGS    - логи Postgres (выход: Ctrl+C)"
    Write-Host "   4. STOP    - остановить (данные сохраняются)"
    Write-Host "   5. RESET   - остановить И удалить все данные БД"  -ForegroundColor Red
    Write-Host "   0. EXIT    - закрыть"
    Write-Host ""
    Write-Host "   pgAdmin  : http://localhost:5050  (admin@local.dev / admin)"
    Write-Host "   Postgres : localhost:5433         (логин/пароль — см. .env.example)"
    Write-Host ""
}

# Печать команды перед выполнением - чтобы запоминать, что под каждой кнопкой
function Invoke-Step {
    param([string]$Label, [scriptblock]$Action)
    Write-Host ""
    Write-Host ">> $Label" -ForegroundColor DarkGray
    & $Action
}

function Wait-Enter {
    Write-Host ""
    Read-Host "Нажми Enter, чтобы вернуться в меню" | Out-Null
}

# ---------- Главное меню (бесконечный цикл) ----------

while ($true) {
    Show-Menu
    $choice = Read-Host "Выбери пункт"

    switch ($choice) {
        "1" {
            Invoke-Step "docker compose up -d" { docker compose up -d }
            Invoke-Step "docker compose ps"    { docker compose ps }
            Wait-Enter
        }
        "2" {
            Invoke-Step "docker compose ps" { docker compose ps }
            Wait-Enter
        }
        "3" {
            Invoke-Step "docker compose logs -f postgres" { docker compose logs -f postgres }
            # после Ctrl+C скрипт завершится целиком - это нормально
        }
        "4" {
            Invoke-Step "docker compose down" { docker compose down }
            Wait-Enter
        }
        "5" {
            Write-Host ""
            Write-Host "   ВНИМАНИЕ: будут удалены ВСЕ данные базы данных!" -ForegroundColor Red
            $confirm = Read-Host "   Введи YES (заглавными) для подтверждения"
            # -ceq = сравнение с учётом регистра: сработает только на точное YES
            if ($confirm -ceq "YES") {
                Invoke-Step "docker compose down -v" { docker compose down -v }
                Write-Host ""
                Write-Host "   Данные удалены. База создастся заново при следующем START (1)."
                Wait-Enter
            }
            else {
                Write-Host "   Отменено." -ForegroundColor Yellow
                Wait-Enter
            }
        }
        "0" {
            exit 0
        }
    }
}

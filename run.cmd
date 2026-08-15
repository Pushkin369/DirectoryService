@echo off
rem Tiny launcher: double-click this file to open the PowerShell menu (run.ps1).
rem ASCII only on purpose - cmd.exe cannot parse Cyrillic reliably.
rem Uses PowerShell 7 (pwsh) if installed, otherwise built-in Windows PowerShell.

where pwsh >nul 2>nul
if %errorlevel%==0 (
    pwsh -NoProfile -ExecutionPolicy Bypass -File "%~dp0run.ps1"
) else (
    powershell -NoProfile -ExecutionPolicy Bypass -File "%~dp0run.ps1"
)

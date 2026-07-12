$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Definition
Set-Location $ScriptDir

Write-Host "Stopping ScriptBee..." -ForegroundColor Cyan
docker compose -f "$ScriptDir\docker-compose.yaml" down

Write-Host ""
Write-Host "ScriptBee has been stopped." -ForegroundColor Green
Write-Host "Your data is preserved in: $ScriptDir\data"
Write-Host "Your plugins are preserved in: $ScriptDir\plugins"

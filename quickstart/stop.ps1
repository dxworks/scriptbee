$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Definition
Set-Location $ScriptDir

$composeFile = Join-Path $ScriptDir "docker-compose.yaml"
if ($args.Count -gt 0) {
    if ($args[0] -eq 'full') {
        $composeFile = Join-Path $ScriptDir "docker-compose-full.yaml"
    } elseif (Test-Path (Join-Path $ScriptDir $args[0])) {
        $composeFile = Join-Path $ScriptDir $args[0]
    }
}

Write-Host "Stopping ScriptBee..." -ForegroundColor Cyan
docker compose -f "$composeFile" down

Write-Host ""
Write-Host "ScriptBee has been stopped." -ForegroundColor Green
Write-Host "Your data is preserved in: $ScriptDir\data"
Write-Host "Your plugins are preserved in: $ScriptDir\plugins"

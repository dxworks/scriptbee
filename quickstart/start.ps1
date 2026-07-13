$COMPOSE_URL = "https://raw.githubusercontent.com/dxworks/scriptbee/main/quickstart/docker-compose.yaml"

$scriptPath = $MyInvocation.MyCommand.Definition
if (-not [string]::IsNullOrEmpty($scriptPath) -and (Test-Path $scriptPath)) {
    $WorkDir = Split-Path -Parent $scriptPath
} else {
    $WorkDir = Join-Path (Get-Location) "scriptbee"
    New-Item -ItemType Directory -Force -Path $WorkDir | Out-Null
    Write-Host "Downloading docker-compose.yaml..." -ForegroundColor Cyan
    Invoke-WebRequest -Uri $COMPOSE_URL -OutFile "$WorkDir\docker-compose.yaml" -UseBasicParsing
}

Set-Location $WorkDir
$env:PWD = $WorkDir

if (-not (Get-Command docker -ErrorAction SilentlyContinue)) {
    Write-Host "ERROR: Docker is not installed. Please install Docker Desktop first:" -ForegroundColor Red
    Write-Host "  https://docs.docker.com/get-docker/" -ForegroundColor Red
    exit 1
}

New-Item -ItemType Directory -Force -Path "data" | Out-Null
New-Item -ItemType Directory -Force -Path "plugins" | Out-Null

# Fetch the version string for the link without downloading the zip
$bundleUrl = "https://github.com/dxworks/scriptbee/releases/latest"
try {
    $releases = Invoke-RestMethod -Uri "https://api.github.com/repos/dxworks/scriptbee/releases" -UseBasicParsing
    $latestBundle = $releases | Where-Object { $_.tag_name -like "bundle@*" } | Select-Object -First 1
    if ($latestBundle) {
        $version = $latestBundle.tag_name -replace "^bundle@", ""
        $bundleUrl = "https://github.com/dxworks/scriptbee/releases/download/bundle%40$version/scriptbee-default-bundle-$version.zip"
    }
} catch {
    # Fallback to general releases if GitHub API fails
    $bundleUrl = "https://github.com/dxworks/scriptbee/releases"
}

Write-Host ""
Write-Host "Starting ScriptBee..." -ForegroundColor Cyan
docker compose -f "$WorkDir\docker-compose.yaml" up -d

Write-Host ""
Write-Host "ScriptBee is starting up!" -ForegroundColor Green
Write-Host "It may take a few seconds for all services to be ready."
Write-Host ""
Write-Host "Open your browser: http://localhost:4201" -ForegroundColor Yellow
Write-Host ""
Write-Host "Default plugin bundle (download and install manually if needed):"
Write-Host "  $bundleUrl" -ForegroundColor Cyan
Write-Host ""
Write-Host "To stop ScriptBee run: docker compose -f `"$WorkDir\docker-compose.yaml`" down"

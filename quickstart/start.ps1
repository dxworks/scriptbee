param(
    [switch]$NoBundle
)

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

function Download-LatestBundle {
    Write-Host "Checking for latest ScriptBee default bundle..." -ForegroundColor Cyan

    try {
        $releases = Invoke-RestMethod -Uri "https://api.github.com/repos/dxworks/scriptbee/releases" -UseBasicParsing
    } catch {
        Write-Host "WARNING: Could not reach GitHub. Skipping bundle download." -ForegroundColor Yellow
        return
    }

    $latestBundle = $releases | Where-Object { $_.tag_name -like "bundle@*" } | Select-Object -First 1

    if (-not $latestBundle) {
        Write-Host "WARNING: Could not determine latest bundle version. Skipping download." -ForegroundColor Yellow
        return
    }

    $version = $latestBundle.tag_name -replace "^bundle@", ""
    $markerDir = Join-Path $WorkDir "plugins\scriptbee-default-bundle@$version"

    if (Test-Path $markerDir) {
        Write-Host "Bundle $version already present - skipping download." -ForegroundColor Green
        return
    }

    $downloadUrl = "https://github.com/dxworks/scriptbee/releases/download/bundle%40$version/scriptbee-default-bundle-$version.zip"
    $zipPath = Join-Path $WorkDir "bundle.zip"

    Write-Host "Downloading bundle $version..." -ForegroundColor Cyan
    Invoke-WebRequest -Uri $downloadUrl -OutFile $zipPath -UseBasicParsing

    Write-Host "Extracting bundle..." -ForegroundColor Cyan
    New-Item -ItemType Directory -Force -Path $markerDir | Out-Null
    Expand-Archive -Path $zipPath -DestinationPath $markerDir -Force
    Remove-Item $zipPath

    Write-Host "Bundle $version installed." -ForegroundColor Green
}

if ($NoBundle) {
    Write-Host "Skipping default bundle download (-NoBundle flag set)." -ForegroundColor Yellow
} else {
    Download-LatestBundle
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
Write-Host "Default plugin bundle (install manually if needed):"
Write-Host "  https://github.com/dxworks/scriptbee/releases/latest/download/scriptbee-default-bundle.zip"
Write-Host ""
Write-Host "To stop ScriptBee run: docker compose -f `"$WorkDir\docker-compose.yaml`" down"

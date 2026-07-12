#!/usr/bin/env bash
set -euo pipefail

COMPOSE_URL="https://raw.githubusercontent.com/dxworks/scriptbee/main/quickstart/docker-compose.yaml"
SKIP_BUNDLE=false

for arg in "$@"; do
    case "$arg" in
        --no-bundle) SKIP_BUNDLE=true ;;
    esac
done

if [ -n "${BASH_SOURCE[0]:-}" ] && [ "${BASH_SOURCE[0]}" != "bash" ] && [ -f "${BASH_SOURCE[0]}" ]; then
    WORK_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
else
    WORK_DIR="$(pwd)/scriptbee"
    mkdir -p "$WORK_DIR"
    echo "Downloading docker-compose.yaml..."
    curl -fsSL "$COMPOSE_URL" -o "$WORK_DIR/docker-compose.yaml"
fi

cd "$WORK_DIR"

if ! command -v docker &>/dev/null; then
    echo "ERROR: Docker is not installed. Please install Docker Desktop first:"
    echo "  https://docs.docker.com/get-docker/"
    exit 1
fi

mkdir -p data plugins

download_latest_bundle() {
    echo "Checking for latest ScriptBee default bundle..."

    local releases_json
    releases_json=$(curl -sf "https://api.github.com/repos/dxworks/scriptbee/releases")

    local version
    version=$(echo "$releases_json" \
        | grep '"tag_name"' \
        | grep '"bundle@' \
        | head -1 \
        | sed 's/.*"bundle@\([^"]*\)".*/\1/')

    if [ -z "$version" ]; then
        echo "WARNING: Could not determine latest bundle version. Skipping download."
        return
    fi

    local marker_dir="plugins/scriptbee-default-bundle@${version}"
    if [ -d "$marker_dir" ]; then
        echo "Bundle ${version} already present - skipping download."
        return
    fi

    local download_url="https://github.com/dxworks/scriptbee/releases/download/bundle%40${version}/scriptbee-default-bundle-${version}.zip"
    echo "Downloading bundle ${version}..."
    curl -fL "$download_url" -o bundle.zip

    echo "Extracting bundle..."
    mkdir -p "$marker_dir"
    unzip -q -o bundle.zip -d "$marker_dir"
    rm -f bundle.zip

    echo "Bundle ${version} installed."
}

if [ "$SKIP_BUNDLE" = "true" ]; then
    echo "Skipping default bundle download (--no-bundle flag set)."
else
    download_latest_bundle
fi

echo ""
echo "Starting ScriptBee..."
docker compose -f "$WORK_DIR/docker-compose.yaml" up -d

echo ""
echo "ScriptBee is starting up!"
echo "It may take a few seconds for all services to be ready."
echo ""
echo "Open your browser: http://localhost:4201"
echo ""
echo "Default plugin bundle (install manually if needed):"
echo "  https://github.com/dxworks/scriptbee/releases/latest/download/scriptbee-default-bundle.zip"
echo ""
echo "To stop ScriptBee run: docker compose -f \"$WORK_DIR/docker-compose.yaml\" down"

#!/usr/bin/env bash
set -euo pipefail

COMPOSE_URL="https://raw.githubusercontent.com/dxworks/scriptbee/main/quickstart/docker-compose.yaml"

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

# Fetch the version string dynamically for the final output link
BUNDLE_URL="https://github.com/dxworks/scriptbee/releases/latest"
if releases_json=$(curl -sf "https://api.github.com/repos/dxworks/scriptbee/releases"); then
    VERSION=$(echo "$releases_json" \
        | grep '"tag_name"' \
        | grep '"bundle@' \
        | head -1 \
        | sed 's/.*"bundle@\([^"]*\)".*/\1/')
    
    if [ -n "$VERSION" ]; then
        BUNDLE_URL="https://github.com/dxworks/scriptbee/releases/download/bundle%40${VERSION}/scriptbee-default-bundle-${VERSION}.zip"
    fi
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
echo "Default plugin bundle (download and install manually if needed):"
echo "  $BUNDLE_URL"
echo ""
echo "To stop ScriptBee run: docker compose -f \"$WORK_DIR/docker-compose.yaml\" down"

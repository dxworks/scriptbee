#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cd "$SCRIPT_DIR"

echo "Stopping ScriptBee..."
docker compose -f "$SCRIPT_DIR/docker-compose.yaml" down

echo ""
echo "ScriptBee has been stopped."
echo "Your data is preserved in: $SCRIPT_DIR/data"
echo "Your plugins are preserved in: $SCRIPT_DIR/plugins"

#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cd "$SCRIPT_DIR"

COMPOSE_FILE="$SCRIPT_DIR/docker-compose.yaml"
if [ "${1-}" = "full" ]; then
  COMPOSE_FILE="$SCRIPT_DIR/docker-compose-full.yaml"
elif [ -n "${1-}" ] && [ -f "$SCRIPT_DIR/${1}" ]; then
  COMPOSE_FILE="$SCRIPT_DIR/${1}"
fi

echo "Stopping ScriptBee..."
docker compose -f "$COMPOSE_FILE" down

echo ""
echo "ScriptBee has been stopped."
echo "Your data is preserved in: $SCRIPT_DIR/data"
echo "Your plugins are preserved in: $SCRIPT_DIR/plugins"

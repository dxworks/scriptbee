#!/bin/bash

if [ -z "$1" ] || [ -z "$2" ]; then
    echo "Usage: $0 <version> <path-to-changelog.md> [output-file]"
    exit 1
fi

VERSION=$1
CHANGELOG_PATH=$2
OUTPUT_FILE=${3:-RELEASE_NOTES.md}

echo "Extracting changelog for version $VERSION from $CHANGELOG_PATH"

awk -v ver="$VERSION" '
    BEGIN {
        # Strip brackets if provided in the version argument
        gsub(/^\[+|\]+$/, "", ver)
        # Escape dots for regex safety
        gsub(/\./, "\\.", ver)
        start_pattern = "^## \\[" ver "\\]"
        end_pattern = "^## \\["
    }
    $0 ~ start_pattern { p=1; next }
    $0 ~ end_pattern && p { p=0; exit }
    p { print }
' "$CHANGELOG_PATH" > "$OUTPUT_FILE"

if [ ! -s "$OUTPUT_FILE" ]; then
    echo " No changelog entry found for version $VERSION"
    exit 1
fi

echo " Changelog extracted successfully to $OUTPUT_FILE"

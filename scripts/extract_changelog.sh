#!/bin/bash

if [ -z "$1" ] || [ -z "$2" ]; then
    echo "Usage: $0 <version> <path-to-changelog.md> [output-file]"
    exit 1
fi

VERSION=$1
CHANGELOG_PATH=$2
OUTPUT_FILE=${3:-RELEASE_NOTES.md}

echo "Extracting changelog for version $VERSION from $CHANGELOG_PATH"

awk "/^## \[$VERSION\]/,/^## \[/" "$CHANGELOG_PATH" | sed '$d' > "$OUTPUT_FILE"

if [ ! -s "$OUTPUT_FILE" ]; then
    echo " No changelog entry found for version $VERSION"
    exit 1
fi

echo " Changelog extracted successfully to $OUTPUT_FILE"

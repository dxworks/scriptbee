#!/usr/bin/env bash

if [ $# -ne 2 ]; then
    echo "Usage: $0 <version> <zip_file_name>"
    exit 1
fi

version=$1
zip_file_name=$2
plugins_folder="./Plugins"
output_folder="./bin"

echo "Creating default plugin bundle for version $version"

sh ./scripts/prepare_default_plugin_bundle.sh "$version" "$plugins_folder" "$output_folder"

echo ""
echo "Creating zip file"

(cd "$output_folder" && zip -r "../$zip_file_name" ./*)

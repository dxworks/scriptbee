#!/usr/bin/env bash

if [ $# -ne 2 ]; then
    echo "Usage: $0 <ui_charts_source_dir> <output_path>"
    exit 1
fi

ui_charts_source_dir=$1
output_path=$2
dist_output="$output_path/dist"

if [ ! -d "$ui_charts_source_dir" ]; then
    echo "UI charts source directory $ui_charts_source_dir does not exist"
    exit 1
fi

echo "Building UI charts plugin..."

(
    cd "$ui_charts_source_dir" || exit 1
    npm ci
    npm run build
)

mkdir -p "$dist_output"

echo "Copying UI charts dist to $dist_output"
cp -r "$ui_charts_source_dir/dist/default-scriptbee-charts/browser/." "$dist_output"

echo ""
echo "Done preparing UI charts plugin"

#!/usr/bin/env bash

if [ $# -ne 3 ]; then
    echo "Usage: $0 <version> <cs_proj_path> <output_path>"
    exit 1
fi

version=$1
cs_proj_path=$2
output_path=$3
bin_path="$output_path/bin"

if [ ! -f "$cs_proj_path" ]; then
    echo "File $cs_proj_path does not exist"
    exit 1
fi

mkdir -p "$bin_path"

echo "Publishing DLLs to $bin_path"

dotnet publish -o "$bin_path" -c Release -p:Version="$version" "$cs_proj_path" 

echo "Copying DLLs to $output_path"

mkdir -p "$output_path"
cp -R "$bin_path"/*.dll "$output_path"

rm -rf "$bin_path"

echo ""
echo "Done preparing $cs_proj_path"

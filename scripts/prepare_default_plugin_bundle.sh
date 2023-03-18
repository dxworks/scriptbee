#!/usr/bin/env bash

if [ $# -ne 3 ]; then
    echo "Usage: $0 <version> <plugins_folder> <output_path>"
    exit 1
fi

version=$1
plugins_folder=$2
output_path=$3
manifest_path="$plugins_folder/manifest.yaml"

if [ ! -d "$plugins_folder" ]; then
    echo "Folder $plugins_folder does not exist"
    exit 1
fi

rm -rf "$output_path"
mkdir -p "$output_path"

echo "Copying manifest to $output_path"
cp "$manifest_path" "$output_path/manifest.yaml"

echo ""
echo "Creating default plugin bundle for version $version"

sh ./scripts/prepare_plugin_folder.sh "$version" "$plugins_folder/HelperFunctions/DxWorks.ScriptBee.Plugin.HelperFunctions.Default/DxWorks.ScriptBee.Plugin.HelperFunctions.Default.csproj" "$output_path/DxWorks.ScriptBee.Plugin.HelperFunctions.Default" && \

sh ./scripts/prepare_plugin_folder.sh "$version" "$plugins_folder/ScriptRunner/DxWorks.ScriptBee.Plugin.ScriptRunner.CSharp/DxWorks.ScriptBee.Plugin.ScriptRunner.CSharp.csproj" "$output_path/DxWorks.ScriptBee.Plugin.ScriptRunner.CSharp" "$plugins_folder/ScriptRunner/DxWorks.ScriptBee.Plugin.ScriptRunner.CSharp/SampleCodes" && \
sh ./scripts/prepare_plugin_folder.sh "$version" "$plugins_folder/ScriptRunner/DxWorks.ScriptBee.Plugin.ScriptRunner.Javascript/DxWorks.ScriptBee.Plugin.ScriptRunner.Javascript.csproj" "$output_path/DxWorks.ScriptBee.Plugin.ScriptRunner.Javascript" "$plugins_folder/ScriptRunner/DxWorks.ScriptBee.Plugin.ScriptRunner.Javascript/SampleCodes" && \
sh ./scripts/prepare_plugin_folder.sh "$version" "$plugins_folder/ScriptRunner/DxWorks.ScriptBee.Plugin.ScriptRunner.Python/DxWorks.ScriptBee.Plugin.ScriptRunner.Python.csproj" "$output_path/DxWorks.ScriptBee.Plugin.ScriptRunner.Python" "$plugins_folder/ScriptRunner/DxWorks.ScriptBee.Plugin.ScriptRunner.Python/SampleCodes"

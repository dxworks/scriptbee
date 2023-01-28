#!/usr/bin/env bash
cd ./DxWorks.ScriptBee.Plugin.Api || exit

dotnet pack -c Release -o ./pack

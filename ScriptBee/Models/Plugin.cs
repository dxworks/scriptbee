using System;
using ScriptBee.Plugin.Manifest;

namespace ScriptBee.Models;

public record Plugin(
    string FolderPath,
    string Id,
    Version Version,
    PluginManifest Manifest
);

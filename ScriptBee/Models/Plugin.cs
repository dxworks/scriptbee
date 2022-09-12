using ScriptBee.Plugin.Manifest;

namespace ScriptBee.Models;

public record Plugin(
    string FolderPath,
    PluginManifest Manifest
);

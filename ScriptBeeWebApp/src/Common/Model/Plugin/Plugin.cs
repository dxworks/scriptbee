using ScriptBee.Domain.Model.Plugin.Manifest;

namespace ScriptBee.Domain.Model.Plugin;

public record Plugin(string FolderPath, string Id, Version Version, PluginManifest Manifest);

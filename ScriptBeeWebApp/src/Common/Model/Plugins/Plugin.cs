using ScriptBee.Domain.Model.Plugins.Manifest;

namespace ScriptBee.Domain.Model.Plugins;

public record Plugin(string FolderPath, PluginId Id, PluginManifest Manifest);

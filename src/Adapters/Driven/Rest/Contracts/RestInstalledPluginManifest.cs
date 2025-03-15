﻿using ScriptBee.Domain.Model.Plugin.Manifest;

namespace ScriptBee.Rest.Contracts;

public class RestInstalledPluginManifest
{
    public string ApiVersion { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string? Author { get; set; }
    public List<RestPluginExtensionPoint> ExtensionPoints { get; set; } = [];

    public PluginManifest Map()
    {
        return new PluginManifest
        {
            ApiVersion = ApiVersion,
            Name = Name,
            Description = Description,
            Author = Author,
            ExtensionPoints = ExtensionPoints
                .Select(ep => ep.Map())
                .Where(ep => ep != null)
                .Cast<PluginExtensionPoint>()
                .ToList(),
        };
    }
}

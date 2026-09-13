using ScriptBee.Domain.Model.Plugins;

namespace DxWorks.ScriptBee.Analysis.Sdk.Abstractions.Errors;

public sealed record InvalidPluginError(PluginId Id);

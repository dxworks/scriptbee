using ScriptBee.Domain.Model.Plugins;

namespace ScriptBee.UseCases.Analysis.Errors;

public sealed record InvalidPluginError(PluginId Id);

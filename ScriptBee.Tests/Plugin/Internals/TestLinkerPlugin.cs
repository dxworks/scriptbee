using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DxWorks.ScriptBee.Plugin.Api;

namespace ScriptBee.Tests.Plugin.Internals;

internal class TestLinkerPlugin : IModelLinker
{
    public Task LinkModel(Dictionary<Tuple<string, string>, Dictionary<string, ScriptBeeModel>> context,
        Dictionary<string, object>? configuration = default, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public string GetName()
    {
        return "";
    }
}

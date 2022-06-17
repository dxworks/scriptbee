using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DxWorks.ScriptBee.Plugin.Api;

public interface IModelLinker
{
    public Task LinkModel(Dictionary<Tuple<string, string>, Dictionary<string, ScriptBeeModel>> context,
        Dictionary<string, object>? configuration = default, CancellationToken cancellationToken = default);

    public string GetName();
}
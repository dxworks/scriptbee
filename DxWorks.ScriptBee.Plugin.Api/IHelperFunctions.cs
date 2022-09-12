﻿using System.Threading;
using System.Threading.Tasks;

namespace DxWorks.ScriptBee.Plugin.Api;

public interface IHelperFunctions
{
    void OnLoadAsync(CancellationToken cancellationToken = default)
    {
    }

    Task OnUnloadAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}

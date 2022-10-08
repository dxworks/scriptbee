using System;
using System.Collections.Generic;
using DxWorks.ScriptBee.Plugin.Api;
using DxWorks.ScriptBee.Plugin.Api.Services;

namespace DxWorks.ScriptBee.Plugin.ScriptRunner.CSharp.Tests;

internal class HelperFunctionsContainer : IHelperFunctionsContainer
{
    private readonly IEnumerable<IHelperFunctions> _helperFunctions;

    public HelperFunctionsContainer(IEnumerable<IHelperFunctions> helperFunctions)
    {
        _helperFunctions = helperFunctions;
    }

    public Dictionary<string, Delegate> GetFunctionsDictionary()
    {
        return new Dictionary<string, Delegate>();
    }

    public IEnumerable<IHelperFunctions> GetFunctions()
    {
        return _helperFunctions;
    }
}

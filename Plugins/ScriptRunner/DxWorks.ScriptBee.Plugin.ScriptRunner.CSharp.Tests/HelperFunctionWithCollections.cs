using System.Collections.Generic;
using DxWorks.ScriptBee.Plugin.Api;

namespace DxWorks.ScriptBee.Plugin.ScriptRunner.CSharp.Tests;

internal class HelperFunctionWithCollections : IHelperFunctions
{
    public void Method<T>(List<T>? list)
    {
    }

    public IDictionary<string, Something> Method()
    {
        return new Dictionary<string, Something>();
    }

    public void Method(List<HashSet<string>> values)
    {
    }
}

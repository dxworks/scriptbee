using System;
using System.Collections.Generic;

namespace DxWorks.ScriptBee.Plugin.Api;

public interface IHelperFunctionsContainer
{
    public Dictionary<string, Delegate> GetFunctionsDictionary();
    
    public IEnumerable<IHelperFunctions> GetFunctions();
}

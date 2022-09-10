using System;
using System.Collections.Generic;
using DxWorks.ScriptBee.Plugin.Api.HelperFunctions;

namespace DxWorks.ScriptBee.Plugin.Api.ScriptRunner;

public interface IHelperFunctionsMapper
{
    public Dictionary<string, Delegate> GetFunctionsDictionary(IHelperFunctions helperFunctions);
}

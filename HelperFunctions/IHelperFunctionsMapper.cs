using System;
using System.Collections.Generic;
using HelperFunctions;

namespace ScriptBeeWebApp.Services;

public interface IHelperFunctionsMapper
{
    public Dictionary<string, Delegate> GetFunctionsDictionary(IHelperFunctions helperFunctions);
}
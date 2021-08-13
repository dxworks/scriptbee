using System;
using System.Collections.Generic;

namespace ScriptBee.Utils
{
    public interface IHelperFunctionsMapper
    {
        IDictionary<string, Delegate> GetFunctionsDictionary();
    }
}
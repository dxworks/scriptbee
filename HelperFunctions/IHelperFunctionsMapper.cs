using System;
using System.Collections.Generic;

namespace HelperFunctions
{
    public interface IHelperFunctionsMapper
    {
        IDictionary<string, Delegate> GetFunctionsDictionary(string projectId);
    }
}
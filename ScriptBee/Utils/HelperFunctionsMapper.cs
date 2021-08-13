using System;
using System.Collections.Generic;

namespace ScriptBee.Utils
{
    public class HelperFunctionsMapper : IHelperFunctionsMapper
    {
        private readonly IDictionary<string, Delegate> _functionsDictionary;

        public HelperFunctionsMapper()
        {
            _functionsDictionary = new Dictionary<string, Delegate>
            {
                {"print", new Action<object>(HelperFunctions.Print)}
            };
        }

        public IDictionary<string, Delegate> GetFunctionsDictionary()
        {
            return _functionsDictionary;
        }
    }
}
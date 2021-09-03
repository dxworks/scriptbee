using System;
using System.Collections.Generic;
using ScriptBeePlugin;

namespace HelperFunctions
{
    public class HelperFunctionsMapper : IHelperFunctionsMapper
    {
        private readonly IDictionary<string, Delegate> _functionsDictionary;

        public HelperFunctionsMapper()
        {
            _functionsDictionary = new Dictionary<string, Delegate>
            {
                {"print", new Action<object>(HelperFunctions.Print)},
                {"get", new Func<Dictionary<Tuple<string, string>, Dictionary<string, ScriptBeeModel>>, Dictionary<string, ScriptBeeModel>>(HelperFunctions.Get)}
            };
        }

        public IDictionary<string, Delegate> GetFunctionsDictionary()
        {
            return _functionsDictionary;
        }
    }
}
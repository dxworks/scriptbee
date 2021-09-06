using System;
using System.Collections.Generic;
using ScriptBeePlugin;

namespace HelperFunctions
{
    public class HelperFunctionsMapper : IHelperFunctionsMapper
    {
        private readonly HelperFunctions _helperFunctions;
        private readonly Dictionary<string, Delegate> _dictionary;

        public HelperFunctionsMapper()
        {
            _helperFunctions = new HelperFunctions();
            _dictionary = new Dictionary<string, Delegate>
            {
                {"print", new Action<object>(_helperFunctions.Print)},
                {
                    "get",
                    new Func<Dictionary<Tuple<string, string>, Dictionary<string, ScriptBeeModel>>,
                        Dictionary<string, ScriptBeeModel>>(_helperFunctions.Get)
                },
                {"printfile", new Action<string, string>(_helperFunctions.WriteToFile)},
            };
        }

        public IDictionary<string, Delegate> GetFunctionsDictionary(string projectId)
        {
            _helperFunctions.ProjectId = projectId;
            return _dictionary;
        }
    }
}
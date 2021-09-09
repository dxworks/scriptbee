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
                    new Func<Dictionary<Tuple<string, string>, Dictionary<string, ScriptBeeModel>>, string, IEnumerable<Dictionary<string, ScriptBeeModel>>>(_helperFunctions.Get)
                },
                {"printf", new Action<string, string>(_helperFunctions.WriteToFile)},
            };
        }

        public IDictionary<string, Delegate> GetFunctionsDictionary(string folderPath)
        {
            _helperFunctions.OutputFolderPath = folderPath;
            return _dictionary;
        }
    }
}
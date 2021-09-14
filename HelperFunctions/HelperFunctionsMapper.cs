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
                {"printFile", new Action<string, string>(_helperFunctions.WriteToFile)},
                {"appendFile", new Action<string, string>(_helperFunctions.AppendToFile)},
                {"appendCsv", new Action<string, List<object>>(_helperFunctions.AppendToCsv)},
                {"exportJson", new Action<string, string>(_helperFunctions.ExportJson)},
                {"exportCsv", new Action<string, List<object>>(_helperFunctions.ExportCsv)},

                {"importJson", new Func<string, object, object>(_helperFunctions.ImportJson)},
                {
                    "get",
                    new Func<Dictionary<Tuple<string, string>, Dictionary<string, ScriptBeeModel>>, string,
                        IEnumerable<Dictionary<string, ScriptBeeModel>>>(_helperFunctions.Get)
                },
            };
        }

        public IDictionary<string, Delegate> GetFunctionsDictionary(string folderPath)
        {
            _helperFunctions.OutputFolderPath = folderPath;
            return _dictionary;
        }

        public HelperFunctions GetHelperFunctions(string folderPath)
        {
            _helperFunctions.OutputFolderPath = folderPath;
            return _helperFunctions;
        }
    }
}
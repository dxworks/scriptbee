using System;
using System.Collections.Generic;

namespace DxWorks.ScriptBee.Plugin.Api.Model;

public interface IContext
{
    public Dictionary<Tuple<string, string>, Dictionary<string, ScriptBeeModel>> Models { get; set; }

    public Dictionary<Tuple<string, string>, Dictionary<string, string>> Tags { get; set; }

    public void SetModel(Tuple<string, string> tuple, Dictionary<string, ScriptBeeModel> objectsDictionary);
    public void RemoveLoaderEntries(string sourceName);

    public List<object> GetClasses();

    public void Clear();
}

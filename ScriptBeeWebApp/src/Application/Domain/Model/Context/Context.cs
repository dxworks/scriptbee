using DxWorks.ScriptBee.Plugin.Api;
using DxWorks.ScriptBee.Plugin.Api.Model;

namespace ScriptBee.Domain.Model.Context;

public class Context : IContext
{
    public Dictionary<
        Tuple<string, string>,
        Dictionary<string, ScriptBeeModel>
    > Models { get; set; } = new();
    public Dictionary<Tuple<string, string>, Dictionary<string, string>> Tags { get; set; } = new();

    public void SetModel(
        Tuple<string, string> tuple,
        Dictionary<string, ScriptBeeModel> objectsDictionary
    )
    {
        Models[tuple] = objectsDictionary;
    }

    public void RemoveLoaderEntries(string sourceName)
    {
        var tuplesToBeRemoved = Models.Keys.Where(tuple => tuple.Item2.Equals(sourceName));
        foreach (var tuple in tuplesToBeRemoved)
        {
            Models.Remove(tuple);
        }
    }

    public List<object> GetClasses()
    {
        var classes = new List<object>();

        foreach (var (_, dictionary) in Models)
        {
            foreach (var (_, model) in dictionary)
            {
                classes.Add(model);
            }
        }

        return classes;
    }

    public void Clear()
    {
        Models.Clear();
        Tags.Clear();
    }
}

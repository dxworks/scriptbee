using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using DxWorks.ScriptBee.Plugin.Api;

namespace DummyPlugin;

public class DummyModelLoader : IModelLoader
{
    public async Task<Dictionary<string, Dictionary<string, ScriptBeeModel>>> LoadModel(List<Stream> fileStreams,
        Dictionary<string, object>? configuration = default, CancellationToken cancellationToken = default)
    {
        var exposedEntities = new Dictionary<string, Dictionary<string, ScriptBeeModel>>();
        var objectsDictionary = new Dictionary<string, ScriptBeeModel>();

        for (var i = 0; i < fileStreams.Count; i++)
        {
            var dummyObject =
                await JsonSerializer.DeserializeAsync<DummyModel>(fileStreams[i], cancellationToken: cancellationToken);
            objectsDictionary.Add(i.ToString(), dummyObject);
        }

        exposedEntities.Add("DummyModel", objectsDictionary);

        return exposedEntities;
    }

    public string GetName()
    {
        return "Dummy";
    }
}
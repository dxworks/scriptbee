using DxWorks.ScriptBee.Plugin.Api.Model;
using MongoDB.Driver;
using ScriptBee.Artifacts.Mongodb.Entity.Script;
using ScriptBee.Domain.Model.ProjectStructure;

namespace ScriptBee.Artifacts.Mongodb.Tests;

public partial class ScriptPersistenceAdapterIntegrationTests
{
    [Fact]
    public async Task CreateScriptWithoutParameters()
    {
        var scriptId = Guid.NewGuid().ToString();
        var script = CreateScript(new ScriptId(scriptId), []);

        await _adapter.Create(script, TestContext.Current.CancellationToken);

        var mongodbScript = await _mongoCollection
            .Find(p => p.Id == scriptId)
            .FirstOrDefaultAsync(cancellationToken: TestContext.Current.CancellationToken);
        mongodbScript.AssertMongodbScript(CreateMongodbScript(scriptId, []));
    }

    [Fact]
    public async Task CreateScriptWithStringParameter()
    {
        var scriptId = Guid.NewGuid().ToString();
        var script = CreateScriptWithParameter(scriptId, ScriptParameter.TypeString, "value");

        await _adapter.Create(script, TestContext.Current.CancellationToken);

        await AssertMongodbScript(scriptId, ScriptParameter.TypeString, "value");
    }

    [Fact]
    public async Task CreateScriptWithBooleanParameter()
    {
        var scriptId = Guid.NewGuid().ToString();
        var script = CreateScriptWithParameter(scriptId, ScriptParameter.TypeBoolean, true);

        await _adapter.Create(script, TestContext.Current.CancellationToken);

        await AssertMongodbScript(scriptId, ScriptParameter.TypeBoolean, true);
    }

    [Fact]
    public async Task CreateScriptWithIntegerParameter()
    {
        var scriptId = Guid.NewGuid().ToString();
        var script = CreateScriptWithParameter(scriptId, ScriptParameter.TypeInteger, 20);

        await _adapter.Create(script, TestContext.Current.CancellationToken);

        await AssertMongodbScript(scriptId, ScriptParameter.TypeInteger, 20);
    }

    [Fact]
    public async Task CreateScriptWithFloatParameter()
    {
        var scriptId = Guid.NewGuid().ToString();
        var script = CreateScriptWithParameter(scriptId, ScriptParameter.TypeFloat, 12.2);

        await _adapter.Create(script, TestContext.Current.CancellationToken);

        await AssertMongodbScript(scriptId, ScriptParameter.TypeFloat, 12.2);
    }

    private async Task AssertMongodbScript(string scriptId, string type, object? value)
    {
        var mongodbScript = await _mongoCollection
            .Find(p => p.Id == scriptId)
            .FirstOrDefaultAsync(cancellationToken: TestContext.Current.CancellationToken);

        mongodbScript.AssertMongodbScript(
            CreateMongodbScript(
                scriptId,
                [
                    new MongodbScriptParameter
                    {
                        Name = "parameter",
                        Type = type,
                        Value = value,
                    },
                ]
            )
        );
    }

    private static Script CreateScriptWithParameter(string scriptId, string type, object? value)
    {
        return CreateScript(
            new ScriptId(scriptId),
            [
                new ScriptParameter
                {
                    Name = "parameter",
                    Type = type,
                    Value = value,
                },
            ]
        );
    }
}

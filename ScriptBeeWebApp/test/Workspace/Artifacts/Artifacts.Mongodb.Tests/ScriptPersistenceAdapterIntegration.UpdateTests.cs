using DxWorks.ScriptBee.Plugin.Api.Model;
using MongoDB.Driver;
using ScriptBee.Artifacts.Mongodb.Entity.Script;
using ScriptBee.Domain.Model.ProjectStructure;

namespace ScriptBee.Artifacts.Mongodb.Tests;

public partial class ScriptPersistenceAdapterIntegrationTests
{
    [Fact]
    public async Task UpdateScriptWithoutParameters()
    {
        var scriptId = Guid.NewGuid().ToString();
        var script = CreateScript(new ScriptId(scriptId), []);
        await _mongoCollection.InsertOneAsync(
            CreateMongodbScript(
                scriptId,
                [
                    new MongodbScriptParameter
                    {
                        Name = "parameter",
                        Type = ScriptParameter.TypeString,
                        Value = "value",
                    },
                ]
            ),
            cancellationToken: TestContext.Current.CancellationToken
        );

        await _adapter.Update(script, TestContext.Current.CancellationToken);

        var mongodbScript = await _mongoCollection
            .Find(p => p.Id == scriptId)
            .FirstOrDefaultAsync(cancellationToken: TestContext.Current.CancellationToken);
        mongodbScript.AssertMongodbScript(CreateMongodbScript(scriptId, []));
    }

    [Fact]
    public async Task UpdateScriptWithStringParameter()
    {
        var scriptId = Guid.NewGuid().ToString();
        var script = CreateScript(
            new ScriptId(scriptId),
            [
                new ScriptParameter
                {
                    Name = "parameter",
                    Type = ScriptParameter.TypeString,
                    Value = "value",
                },
            ]
        );
        await _mongoCollection.InsertOneAsync(
            CreateMongodbScript(scriptId, []),
            cancellationToken: TestContext.Current.CancellationToken
        );

        await _adapter.Update(script, TestContext.Current.CancellationToken);

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
                        Type = ScriptParameter.TypeString,
                        Value = "value",
                    },
                ]
            )
        );
    }

    [Fact]
    public async Task UpdateScriptWithBooleanParameter()
    {
        var scriptId = Guid.NewGuid().ToString();
        var script = CreateScript(
            new ScriptId(scriptId),
            [
                new ScriptParameter
                {
                    Name = "parameter",
                    Type = ScriptParameter.TypeBoolean,
                    Value = true,
                },
            ]
        );
        await _mongoCollection.InsertOneAsync(
            CreateMongodbScript(scriptId, []),
            cancellationToken: TestContext.Current.CancellationToken
        );

        await _adapter.Update(script, TestContext.Current.CancellationToken);

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
                        Type = ScriptParameter.TypeBoolean,
                        Value = true,
                    },
                ]
            )
        );
    }

    [Fact]
    public async Task UpdateScriptWithIntegerParameter()
    {
        var scriptId = Guid.NewGuid().ToString();
        var script = CreateScript(
            new ScriptId(scriptId),
            [
                new ScriptParameter
                {
                    Name = "parameter",
                    Type = ScriptParameter.TypeInteger,
                    Value = 20,
                },
            ]
        );
        await _mongoCollection.InsertOneAsync(
            CreateMongodbScript(scriptId, []),
            cancellationToken: TestContext.Current.CancellationToken
        );

        await _adapter.Update(script, TestContext.Current.CancellationToken);

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
                        Type = ScriptParameter.TypeInteger,
                        Value = 20,
                    },
                ]
            )
        );
    }

    [Fact]
    public async Task UpdateScriptWithFloatParameter()
    {
        var scriptId = Guid.NewGuid().ToString();
        var script = CreateScript(
            new ScriptId(scriptId),
            [
                new ScriptParameter
                {
                    Name = "parameter",
                    Type = ScriptParameter.TypeFloat,
                    Value = 12.2,
                },
            ]
        );
        await _mongoCollection.InsertOneAsync(
            CreateMongodbScript(scriptId, []),
            cancellationToken: TestContext.Current.CancellationToken
        );

        await _adapter.Update(script, TestContext.Current.CancellationToken);

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
                        Type = ScriptParameter.TypeFloat,
                        Value = 12.2,
                    },
                ]
            )
        );
    }
}

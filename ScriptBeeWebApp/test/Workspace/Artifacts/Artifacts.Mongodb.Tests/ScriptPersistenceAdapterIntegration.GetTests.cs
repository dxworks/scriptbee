using DxWorks.ScriptBee.Plugin.Api.Model;
using ScriptBee.Artifacts.Mongodb.Entity.Script;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.ProjectStructure;

namespace ScriptBee.Artifacts.Mongodb.Tests;

public partial class ScriptPersistenceAdapterIntegrationTests
{
    [Fact]
    public async Task GivenNoExistingScript_Get_ReturnScriptDoesNotExistsError()
    {
        var scriptId = Guid.NewGuid().ToString();

        var result = await _adapter.Get(
            new ScriptId(scriptId),
            TestContext.Current.CancellationToken
        );

        result.AsT1.ShouldBe(new ScriptDoesNotExistsError(new ScriptId(scriptId)));
    }

    [Fact]
    public async Task GetScriptWithoutParameters()
    {
        var scriptId = Guid.NewGuid().ToString();
        await _mongoCollection.InsertOneAsync(
            CreateMongodbScript(scriptId, []),
            cancellationToken: TestContext.Current.CancellationToken
        );

        var result = await _adapter.Get(
            new ScriptId(scriptId),
            TestContext.Current.CancellationToken
        );

        result.AsT0.AssertScript(CreateScript(new ScriptId(scriptId), []));
    }

    [Fact]
    public async Task GetScriptWithStringParameter()
    {
        var scriptId = Guid.NewGuid().ToString();
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

        var result = await _adapter.Get(
            new ScriptId(scriptId),
            TestContext.Current.CancellationToken
        );

        result.AsT0.AssertScript(
            CreateScript(
                new ScriptId(scriptId),
                [
                    new ScriptParameter
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
    public async Task GetScriptWithBooleanParameter()
    {
        var scriptId = Guid.NewGuid().ToString();
        await _mongoCollection.InsertOneAsync(
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
            ),
            cancellationToken: TestContext.Current.CancellationToken
        );

        var result = await _adapter.Get(
            new ScriptId(scriptId),
            TestContext.Current.CancellationToken
        );

        result.AsT0.AssertScript(
            CreateScript(
                new ScriptId(scriptId),
                [
                    new ScriptParameter
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
    public async Task GetScriptWithIntegerParameter()
    {
        var scriptId = Guid.NewGuid().ToString();
        await _mongoCollection.InsertOneAsync(
            CreateMongodbScript(
                scriptId,
                [
                    new MongodbScriptParameter
                    {
                        Name = "parameter",
                        Type = ScriptParameter.TypeInteger,
                        Value = 8,
                    },
                ]
            ),
            cancellationToken: TestContext.Current.CancellationToken
        );

        var result = await _adapter.Get(
            new ScriptId(scriptId),
            TestContext.Current.CancellationToken
        );

        result.AsT0.AssertScript(
            CreateScript(
                new ScriptId(scriptId),
                [
                    new ScriptParameter
                    {
                        Name = "parameter",
                        Type = ScriptParameter.TypeInteger,
                        Value = 8,
                    },
                ]
            )
        );
    }

    [Fact]
    public async Task GetScriptWithFloatParameter()
    {
        var scriptId = Guid.NewGuid().ToString();
        await _mongoCollection.InsertOneAsync(
            CreateMongodbScript(
                scriptId,
                [
                    new MongodbScriptParameter
                    {
                        Name = "parameter",
                        Type = ScriptParameter.TypeFloat,
                        Value = 17.9,
                    },
                ]
            ),
            cancellationToken: TestContext.Current.CancellationToken
        );

        var result = await _adapter.Get(
            new ScriptId(scriptId),
            TestContext.Current.CancellationToken
        );

        result.AsT0.AssertScript(
            CreateScript(
                new ScriptId(scriptId),
                [
                    new ScriptParameter
                    {
                        Name = "parameter",
                        Type = ScriptParameter.TypeFloat,
                        Value = 17.9,
                    },
                ]
            )
        );
    }
}

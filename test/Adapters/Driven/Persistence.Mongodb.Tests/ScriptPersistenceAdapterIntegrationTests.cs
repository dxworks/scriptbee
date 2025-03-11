using DxWorks.ScriptBee.Plugin.Api.Model;
using MongoDB.Driver;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.ProjectStructure;
using ScriptBee.Persistence.Mongodb.Entity.Script;
using ScriptBee.Persistence.Mongodb.Repository;

namespace ScriptBee.Persistence.Mongodb.Tests;

public class ScriptPersistenceAdapterIntegrationTests : IClassFixture<MongoDbFixture>
{
    private readonly ScriptPersistenceAdapter _adapter;
    private readonly IMongoCollection<MongodbScript> _mongoCollection;

    public ScriptPersistenceAdapterIntegrationTests(MongoDbFixture fixture)
    {
        _mongoCollection = fixture.GetCollection<MongodbScript>("Scripts");
        _adapter = new ScriptPersistenceAdapter(
            new MongoRepository<MongodbScript>(_mongoCollection)
        );
    }

    [Fact]
    public async Task CreateScriptWithoutParameters()
    {
        const string scriptId = "8447d113-2cf5-4eab-afd4-8e691778ff96";
        var script = CreateScript(new ScriptId(scriptId), []);

        await _adapter.Create(script, CancellationToken.None);

        var mongodbScript = await _mongoCollection
            .Find(p => p.Id == scriptId)
            .FirstOrDefaultAsync();
        mongodbScript.AssertMongodbScript(CreateMongodbScript(scriptId, []));
    }

    [Fact]
    public async Task CreateScriptWithStringParameter()
    {
        const string scriptId = "b5874db7-5351-4598-b958-b3cd0f799f94";
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

        await _adapter.Create(script, CancellationToken.None);

        var mongodbScript = await _mongoCollection
            .Find(p => p.Id == scriptId)
            .FirstOrDefaultAsync();
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
    public async Task CreateScriptWithBooleanParameter()
    {
        const string scriptId = "f0559010-2c56-4676-a460-75a854448f82";
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

        await _adapter.Create(script, CancellationToken.None);

        var mongodbScript = await _mongoCollection
            .Find(p => p.Id == scriptId)
            .FirstOrDefaultAsync();
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
    public async Task CreateScriptWithIntegerParameter()
    {
        const string scriptId = "ee1cfa99-16ff-4955-bc1d-3ef8266c0957";
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

        await _adapter.Create(script, CancellationToken.None);

        var mongodbScript = await _mongoCollection
            .Find(p => p.Id == scriptId)
            .FirstOrDefaultAsync();
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
    public async Task CreateScriptWithFloatParameter()
    {
        const string scriptId = "c8ccbfc2-638a-4f7f-aaa4-a166ff032324";
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

        await _adapter.Create(script, CancellationToken.None);

        var mongodbScript = await _mongoCollection
            .Find(p => p.Id == scriptId)
            .FirstOrDefaultAsync();
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

    private static Script CreateScript(ScriptId id, IEnumerable<ScriptParameter> parameters)
    {
        return new Script(
            id,
            ProjectId.Create("id"),
            "script.cs",
            "path",
            "absolute",
            new ScriptLanguage("csharp", ".cs"),
            parameters
        );
    }

    private static MongodbScript CreateMongodbScript(
        string id,
        IEnumerable<MongodbScriptParameter> parameters
    )
    {
        return new MongodbScript
        {
            Id = id,
            ProjectId = "id",
            Name = "script.cs",
            FilePath = "path",
            AbsoluteFilePath = "absolute",
            ScriptLanguageName = "csharp",
            Parameters = parameters,
        };
    }
}

public static class MongodbScriptAssertionsExtensions
{
    public static void AssertMongodbScript(this MongodbScript actual, MongodbScript expected)
    {
        actual.Id.ShouldBe(expected.Id);
        actual.ProjectId.ShouldBe(expected.ProjectId);
        actual.Name.ShouldBe(expected.Name);
        actual.FilePath.ShouldBe(expected.FilePath);
        actual.AbsoluteFilePath.ShouldBe(expected.AbsoluteFilePath);
        actual.ScriptLanguageName.ShouldBe(expected.ScriptLanguageName);
        actual.Parameters.ToList().ShouldBeEquivalentTo(expected.Parameters.ToList());
    }
}

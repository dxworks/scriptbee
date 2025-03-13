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

    #region Create Script

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

    #endregion

    #region Get Script

    [Fact]
    public async Task GivenNoExistingScript_Get_ReturnScriptDoesNotExistsError()
    {
        const string scriptId = "bccab17e-bfc1-4209-aa12-75cabd307299";

        var result = await _adapter.Get(new ScriptId(scriptId), CancellationToken.None);

        result.AsT1.ShouldBe(new ScriptDoesNotExistsError(new ScriptId(scriptId)));
    }

    [Fact]
    public async Task GetScriptWithoutParameters()
    {
        const string scriptId = "fd2c1c8c-eb1a-4e6c-824c-b0e23b061c36";
        await _mongoCollection.InsertOneAsync(CreateMongodbScript(scriptId, []));

        var result = await _adapter.Get(new ScriptId(scriptId), CancellationToken.None);

        result.AsT0.AssertScript(CreateScript(new ScriptId(scriptId), []));
    }

    [Fact]
    public async Task GetScriptWithStringParameter()
    {
        const string scriptId = "17938834-37f3-4316-a533-26ff079d156e";
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
            )
        );

        var result = await _adapter.Get(new ScriptId(scriptId), CancellationToken.None);

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
        const string scriptId = "1ae3ee68-2b4d-4ee2-a1c2-09784b68f1fd";
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
            )
        );

        var result = await _adapter.Get(new ScriptId(scriptId), CancellationToken.None);

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
        const string scriptId = "c7757e1d-54ce-45e0-8728-22dc78dcd578";
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
            )
        );

        var result = await _adapter.Get(new ScriptId(scriptId), CancellationToken.None);

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
        const string scriptId = "fcf57f00-a1ba-4813-9d50-53e17894756a";
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
            )
        );

        var result = await _adapter.Get(new ScriptId(scriptId), CancellationToken.None);

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

    #endregion


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
            ScriptLanguage = new MongodbScriptLanguage { Name = "csharp", Extension = ".cs" },
            Parameters = parameters,
        };
    }
}

public static class ScriptAssertionsExtensions
{
    public static void AssertMongodbScript(this MongodbScript actual, MongodbScript expected)
    {
        actual.Id.ShouldBe(expected.Id);
        actual.ProjectId.ShouldBe(expected.ProjectId);
        actual.Name.ShouldBe(expected.Name);
        actual.FilePath.ShouldBe(expected.FilePath);
        actual.AbsoluteFilePath.ShouldBe(expected.AbsoluteFilePath);
        actual.ScriptLanguage.Name.ShouldBe(expected.ScriptLanguage.Name);
        actual.ScriptLanguage.Extension.ShouldBe(expected.ScriptLanguage.Extension);
        actual.Parameters.ToList().ShouldBeEquivalentTo(expected.Parameters.ToList());
    }

    public static void AssertScript(this Script actual, Script expected)
    {
        actual.Id.ShouldBe(expected.Id);
        actual.ProjectId.ShouldBe(expected.ProjectId);
        actual.Name.ShouldBe(expected.Name);
        actual.FilePath.ShouldBe(expected.FilePath);
        actual.AbsoluteFilePath.ShouldBe(expected.AbsoluteFilePath);
        actual.ScriptLanguage.ShouldBe(expected.ScriptLanguage);
        actual.Parameters.ToList().ShouldBeEquivalentTo(expected.Parameters.ToList());
    }
}

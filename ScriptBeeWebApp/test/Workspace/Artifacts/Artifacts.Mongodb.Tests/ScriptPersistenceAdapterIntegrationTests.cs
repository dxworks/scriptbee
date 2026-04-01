using DxWorks.ScriptBee.Plugin.Api.Model;
using MongoDB.Driver;
using ScriptBee.Artifacts.Mongodb.Entity.Script;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.ProjectStructure;
using ScriptBee.Persistence.Mongodb.Repository;
using ScriptBee.Tests.Common.Mongodb;

namespace ScriptBee.Artifacts.Mongodb.Tests;

public class ScriptPersistenceAdapterIntegrationTests : IClassFixture<MongoDbFixture>
{
    private readonly ScriptsPersistenceAdapter _adapter;
    private readonly IMongoCollection<MongodbScript> _mongoCollection;

    public ScriptPersistenceAdapterIntegrationTests(MongoDbFixture fixture)
    {
        _mongoCollection = fixture.GetCollection<MongodbScript>("Scripts");
        _adapter = new ScriptsPersistenceAdapter(
            new MongoRepository<MongodbScript>(_mongoCollection)
        );
    }

    #region Create Script

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

        await _adapter.Create(script, TestContext.Current.CancellationToken);

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
    public async Task CreateScriptWithBooleanParameter()
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

        await _adapter.Create(script, TestContext.Current.CancellationToken);

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
    public async Task CreateScriptWithIntegerParameter()
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

        await _adapter.Create(script, TestContext.Current.CancellationToken);

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
    public async Task CreateScriptWithFloatParameter()
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

        await _adapter.Create(script, TestContext.Current.CancellationToken);

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

    #endregion

    #region Get Script

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

    #endregion

    #region Get All Scripts

    [Fact]
    public async Task GetAllScripts()
    {
        var scriptId = Guid.NewGuid().ToString();
        await _mongoCollection.InsertOneAsync(
            CreateMongodbScript(scriptId, []),
            cancellationToken: TestContext.Current.CancellationToken
        );

        var result = await _adapter.GetAll(
            ProjectId.FromValue("id"),
            TestContext.Current.CancellationToken
        );

        result
            .Single(s => s.Id.Value.ToString() == scriptId)
            .AssertScript(CreateScript(new ScriptId(scriptId), []));
    }

    #endregion

    #region Update Script

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

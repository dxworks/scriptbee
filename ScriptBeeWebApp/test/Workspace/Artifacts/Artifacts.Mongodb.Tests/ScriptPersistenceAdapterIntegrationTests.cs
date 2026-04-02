using DxWorks.ScriptBee.Plugin.Api.Model;
using MongoDB.Driver;
using ScriptBee.Artifacts.Mongodb.Entity.Script;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.ProjectStructure;
using ScriptBee.Persistence.Mongodb.Repository;
using ScriptBee.Tests.Common.Mongodb;

namespace ScriptBee.Artifacts.Mongodb.Tests;

public partial class ScriptPersistenceAdapterIntegrationTests : IClassFixture<MongoDbFixture>
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

    private static Script CreateScript(ScriptId id, IEnumerable<ScriptParameter> parameters)
    {
        return new Script(
            id,
            ProjectId.Create("id"),
            new ProjectStructureFile("path"),
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
            FilePath = "path",
            ScriptLanguage = new MongodbScriptLanguage { Name = "csharp", Extension = ".cs" },
            Parameters = parameters,
            Type = MongodbScriptType.File,
            ChildrenIds = null,
        };
    }
}

public static class ScriptAssertionsExtensions
{
    public static void AssertMongodbScript(this MongodbScript actual, MongodbScript expected)
    {
        actual.Id.ShouldBe(expected.Id);
        actual.ProjectId.ShouldBe(expected.ProjectId);
        actual.FilePath.ShouldBe(expected.FilePath);
        actual.ScriptLanguage!.Name.ShouldBe(expected.ScriptLanguage!.Name);
        actual.ScriptLanguage.Extension.ShouldBe(expected.ScriptLanguage.Extension);
        actual.Parameters!.ToList().ShouldBeEquivalentTo(expected.Parameters!.ToList());
    }

    public static void AssertScript(this Script actual, Script expected)
    {
        actual.Id.ShouldBe(expected.Id);
        actual.ProjectId.ShouldBe(expected.ProjectId);
        actual.File.Path.ShouldBe(expected.File.Path);
        actual.ScriptLanguage.ShouldBe(expected.ScriptLanguage);
        actual.Parameters.ToList().ShouldBeEquivalentTo(expected.Parameters.ToList());
    }
}

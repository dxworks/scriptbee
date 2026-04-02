using DxWorks.ScriptBee.Plugin.Api.Model;
using MongoDB.Driver;
using NSubstitute;
using ScriptBee.Artifacts.Mongodb.Entity.Script;
using ScriptBee.Common;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.ProjectStructure;
using ScriptBee.Persistence.Mongodb.Repository;
using ScriptBee.Tests.Common.Mongodb;

namespace ScriptBee.Artifacts.Mongodb.Tests;

public partial class ScriptPersistenceAdapterIntegrationTests : IClassFixture<MongoDbFixture>
{
    private readonly ScriptsPersistenceAdapter _adapter;
    private readonly IGuidProvider _guidProvider = Substitute.For<IGuidProvider>();
    private readonly IMongoCollection<MongodbScript> _mongoCollection;

    public ScriptPersistenceAdapterIntegrationTests(MongoDbFixture fixture)
    {
        _mongoCollection = fixture.GetCollection<MongodbScript>("Scripts");
        _adapter = new ScriptsPersistenceAdapter(
            new MongoRepository<MongodbScript>(_mongoCollection),
            _guidProvider
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
        IEnumerable<MongodbScriptParameter> parameters,
        string filePath = "path"
    )
    {
        return new MongodbScript
        {
            Id = id,
            ProjectId = "id",
            FilePath = filePath,
            ScriptLanguage = new MongodbScriptLanguage { Name = "csharp", Extension = ".cs" },
            Parameters = parameters,
            Type = MongodbScriptType.File,
            ChildrenIds = null,
        };
    }

    private static MongodbScript CreateMongodbScriptFolder(
        string id,
        IEnumerable<string> childrenIds,
        string filePath
    )
    {
        return new MongodbScript
        {
            Id = id,
            ProjectId = "id",
            FilePath = filePath,
            ScriptLanguage = null,
            Parameters = null,
            Type = MongodbScriptType.Folder,
            ChildrenIds = childrenIds,
        };
    }
}

public static class ScriptAssertionsExtensions
{
    extension(MongodbScript actual)
    {
        public void AssertMongodbScript(MongodbScript expected)
        {
            actual.Id.ShouldBe(expected.Id);
            actual.ProjectId.ShouldBe(expected.ProjectId);
            actual.FilePath.ShouldBe(expected.FilePath);
            actual.ScriptLanguage!.Name.ShouldBe(expected.ScriptLanguage!.Name);
            actual.ScriptLanguage.Extension.ShouldBe(expected.ScriptLanguage.Extension);
            actual.Parameters!.ToList().ShouldBeEquivalentTo(expected.Parameters!.ToList());
        }

        public void AssertMongodbScriptFolder(MongodbScript expected)
        {
            actual.Id.ShouldBe(expected.Id);
            actual.ProjectId.ShouldBe(expected.ProjectId);
            actual.FilePath.ShouldBe(expected.FilePath);
            actual.ScriptLanguage.ShouldBeNull();
            actual.Parameters.ShouldBeNull();
            actual.ChildrenIds!.ToList().ShouldBeEquivalentTo(expected.ChildrenIds!.ToList());
        }
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

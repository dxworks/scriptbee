using DxWorks.ScriptBee.Plugin.Api.Model;
using MongoDB.Driver;
using NSubstitute;
using ScriptBee.Artifacts.Mongodb.Entity.Script;
using ScriptBee.Domain.Model.ProjectStructure;

namespace ScriptBee.Artifacts.Mongodb.Tests;

public partial class ScriptPersistenceAdapterIntegrationTests
{
    [Fact]
    public async Task CreateScriptWithOneParentFolder_ShouldCreateFolder()
    {
        // Arrange
        var scriptId = Guid.NewGuid().ToString();
        var folderId = Guid.NewGuid();
        var script = CreateScript(new ScriptId(scriptId), []) with
        {
            File = new ProjectStructureFile("folder/script.cs"),
        };
        _guidProvider.NewGuid().Returns(folderId);

        // Act
        await _adapter.Create(script, TestContext.Current.CancellationToken);

        // Assert
        var mongodbScript = await _mongoCollection
            .Find(p => p.Id == scriptId)
            .FirstOrDefaultAsync(cancellationToken: TestContext.Current.CancellationToken);
        mongodbScript.AssertMongodbScript(CreateMongodbScript(scriptId, [], "folder/script.cs"));

        var folder = await _mongoCollection
            .Find(p => p.Id == folderId.ToString())
            .FirstOrDefaultAsync(cancellationToken: TestContext.Current.CancellationToken);
        folder.AssertMongodbScriptFolder(
            CreateMongodbScriptFolder(folderId.ToString(), [scriptId], "folder")
        );
    }

    [Fact]
    public async Task CreateScriptWithMultipleParentFolders_ShouldCreateAllFolders()
    {
        // Arrange
        var scriptId = Guid.NewGuid().ToString();
        var folderId1 = Guid.NewGuid();
        var folderId2 = Guid.NewGuid();
        var script = CreateScript(new ScriptId(scriptId), []) with
        {
            File = new ProjectStructureFile("folder1/folder2/script.cs"),
        };
        _guidProvider.NewGuid().Returns(folderId2, folderId1);

        // Act
        await _adapter.Create(script, TestContext.Current.CancellationToken);

        // Assert
        var mongodbScript = await _mongoCollection
            .Find(p => p.Id == scriptId)
            .FirstOrDefaultAsync(cancellationToken: TestContext.Current.CancellationToken);
        mongodbScript.AssertMongodbScript(
            CreateMongodbScript(scriptId, [], "folder1/folder2/script.cs")
        );

        var folder1 = await _mongoCollection
            .Find(p => p.Id == folderId1.ToString())
            .FirstOrDefaultAsync(cancellationToken: TestContext.Current.CancellationToken);
        folder1.AssertMongodbScriptFolder(
            CreateMongodbScriptFolder(folderId1.ToString(), [folderId2.ToString()], "folder1")
        );

        var folder2 = await _mongoCollection
            .Find(p => p.Id == folderId2.ToString())
            .FirstOrDefaultAsync(cancellationToken: TestContext.Current.CancellationToken);
        folder2.AssertMongodbScriptFolder(
            CreateMongodbScriptFolder(folderId2.ToString(), [scriptId], "folder1/folder2")
        );
    }

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

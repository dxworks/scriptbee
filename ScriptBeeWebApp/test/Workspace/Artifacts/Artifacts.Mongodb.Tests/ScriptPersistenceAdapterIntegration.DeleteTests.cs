using MongoDB.Driver;
using ScriptBee.Domain.Model.ProjectStructure;

namespace ScriptBee.Artifacts.Mongodb.Tests;

public partial class ScriptPersistenceAdapterIntegrationTests
{
    [Fact]
    public async Task Delete_ShouldReturnNullWhenNoEntryNeedsDeletion()
    {
        // Arrange
        var scriptId = Guid.NewGuid().ToString();

        // Act
        var entry = await _adapter.Delete(
            new ScriptId(scriptId),
            TestContext.Current.CancellationToken
        );

        // Assert
        entry.ShouldBeNull();
        var mongodbScript = await _mongoCollection
            .Find(p => p.Id == scriptId)
            .FirstOrDefaultAsync(cancellationToken: TestContext.Current.CancellationToken);
        mongodbScript.ShouldBeNull();
    }

    [Fact]
    public async Task DeleteScript_ShouldReturnScript()
    {
        // Arrange
        var scriptId = Guid.NewGuid().ToString();
        await _mongoCollection.InsertOneAsync(
            CreateMongodbScript(scriptId, []),
            cancellationToken: TestContext.Current.CancellationToken
        );

        // Act
        var entry = await _adapter.Delete(
            new ScriptId(scriptId),
            TestContext.Current.CancellationToken
        );

        // Assert
        entry.ShouldNotBeNull();
        entry.Id.ShouldBe(new ScriptId(scriptId));
        var mongodbScript = await _mongoCollection
            .Find(p => p.Id == scriptId)
            .FirstOrDefaultAsync(cancellationToken: TestContext.Current.CancellationToken);
        mongodbScript.ShouldBeNull();
    }

    [Fact]
    public async Task DeleteFolderWithoutChildren_ShouldReturnFolder()
    {
        // Arrange
        var scriptId = Guid.NewGuid().ToString();
        await _mongoCollection.InsertOneAsync(
            CreateMongodbScriptFolder(scriptId, [], "zzz"),
            cancellationToken: TestContext.Current.CancellationToken
        );

        // Act
        var entry = await _adapter.Delete(
            new ScriptId(scriptId),
            TestContext.Current.CancellationToken
        );

        // Assert
        entry.ShouldNotBeNull();
        entry.Id.ShouldBe(new ScriptId(scriptId));
        var mongodbScript = await _mongoCollection
            .Find(p => p.Id == scriptId)
            .FirstOrDefaultAsync(cancellationToken: TestContext.Current.CancellationToken);
        mongodbScript.ShouldBeNull();
    }

    [Fact]
    public async Task DeleteFolderWithChildren_ShouldReturnFolderAndAllChildren()
    {
        // Arrange
        var folderId = Guid.NewGuid().ToString();
        var scriptId = Guid.NewGuid().ToString();
        await _mongoCollection.InsertOneAsync(
            CreateMongodbScript(scriptId, []),
            cancellationToken: TestContext.Current.CancellationToken
        );
        await _mongoCollection.InsertOneAsync(
            CreateMongodbScriptFolder(folderId, [scriptId], "f-path"),
            cancellationToken: TestContext.Current.CancellationToken
        );

        // Act
        var entry = await _adapter.Delete(
            new ScriptId(folderId),
            TestContext.Current.CancellationToken
        );

        // Assert
        entry.ShouldNotBeNull();
        entry.Id.ShouldBe(new ScriptId(folderId));
        var mongodbFolder = await _mongoCollection
            .Find(p => p.Id == folderId)
            .FirstOrDefaultAsync(cancellationToken: TestContext.Current.CancellationToken);
        mongodbFolder.ShouldBeNull();

        var mongodbScript = await _mongoCollection
            .Find(p => p.Id == scriptId)
            .FirstOrDefaultAsync(cancellationToken: TestContext.Current.CancellationToken);
        mongodbScript.ShouldBeNull();
    }
}

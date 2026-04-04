using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.ProjectStructure;

namespace ScriptBee.Artifacts.Mongodb.Tests;

public partial class ScriptPersistenceAdapterIntegrationTests
{
    [Fact]
    public async Task ListRootEntries_GivenNoProjectsForProjectId_ShouldReturnEmptyPage()
    {
        // Arrange
        var projectId = ProjectId.FromValue(Guid.NewGuid().ToString());

        // Act
        var page = await _adapter.ListRootEntries(
            projectId,
            0,
            1,
            TestContext.Current.CancellationToken
        );

        // Assert
        page.Offset.ShouldBe(0);
        page.Limit.ShouldBe(1);
        page.TotalCount.ShouldBe(0);
        page.Data.ShouldBeEmpty();
    }

    [Fact]
    public async Task ListRootEntries_GivenMultipleEntries_ShouldReturnOnlyEntriesFromRootFolder()
    {
        // Arrange
        var projectId = ProjectId.FromValue(Guid.NewGuid().ToString());
        var scriptId1 = Guid.NewGuid().ToString();
        var scriptId2 = Guid.NewGuid().ToString();
        var scriptId3 = Guid.NewGuid().ToString();
        await _mongoCollection.InsertOneAsync(
            CreateMongodbScript(scriptId1, [], "path/file.txt", projectId.Value),
            cancellationToken: TestContext.Current.CancellationToken
        );
        await _mongoCollection.InsertOneAsync(
            CreateMongodbScript(scriptId2, [], "root.txt", projectId.Value),
            cancellationToken: TestContext.Current.CancellationToken
        );
        await _mongoCollection.InsertOneAsync(
            CreateMongodbScriptFolder(scriptId3, [], "root", projectId.Value),
            cancellationToken: TestContext.Current.CancellationToken
        );
        await _mongoCollection.InsertOneAsync(
            CreateMongodbScript(Guid.NewGuid().ToString(), [], "root"),
            cancellationToken: TestContext.Current.CancellationToken
        );

        // Act
        var page = await _adapter.ListRootEntries(
            projectId,
            0,
            4,
            TestContext.Current.CancellationToken
        );

        // Assert
        page.Offset.ShouldBe(0);
        page.Limit.ShouldBe(4);
        page.TotalCount.ShouldBe(2);
        var entries = page.Data.ToList();
        entries.Count.ShouldBe(2);
        entries[0]
            .AssertProjectStructureEntry(
                CreateScript(new ScriptId(scriptId2), [], "root.txt", projectId.ToString())
            );
        entries[1]
            .AssertProjectStructureEntry(
                CreateScriptFolder(new ScriptId(scriptId3), "root", projectId.ToString(), [])
            );
    }

    [Fact]
    public async Task ListRootEntries_GivenMultipleEntries_ShouldReturnOnlyEntriesFromRootFolderWithinLimitAndOffset()
    {
        // Arrange
        var projectId = ProjectId.FromValue(Guid.NewGuid().ToString());
        var scriptId1 = Guid.NewGuid().ToString();
        var scriptId2 = Guid.NewGuid().ToString();
        await _mongoCollection.InsertOneAsync(
            CreateMongodbScript(Guid.NewGuid().ToString(), [], "path", projectId.Value),
            cancellationToken: TestContext.Current.CancellationToken
        );
        await _mongoCollection.InsertOneAsync(
            CreateMongodbScript(scriptId1, [], "root.txt", projectId.Value),
            cancellationToken: TestContext.Current.CancellationToken
        );
        await _mongoCollection.InsertOneAsync(
            CreateMongodbScriptFolder(scriptId2, [], "root", projectId.Value),
            cancellationToken: TestContext.Current.CancellationToken
        );
        await _mongoCollection.InsertOneAsync(
            CreateMongodbScript(Guid.NewGuid().ToString(), [], "root", projectId.Value),
            cancellationToken: TestContext.Current.CancellationToken
        );

        // Act
        var page = await _adapter.ListRootEntries(
            projectId,
            1,
            2,
            TestContext.Current.CancellationToken
        );

        // Assert
        page.Offset.ShouldBe(1);
        page.Limit.ShouldBe(2);
        page.TotalCount.ShouldBe(4);
        var entries = page.Data.ToList();
        entries.Count.ShouldBe(2);
        entries[0]
            .AssertProjectStructureEntry(
                CreateScript(new ScriptId(scriptId1), [], "root.txt", projectId.ToString())
            );
        entries[1]
            .AssertProjectStructureEntry(
                CreateScriptFolder(new ScriptId(scriptId2), "root", projectId.ToString(), [])
            );
    }
}

using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.ProjectStructure;

namespace ScriptBee.Artifacts.Mongodb.Tests;

public partial class ScriptPersistenceAdapterIntegrationTests
{
    [Fact]
    public async Task ListEntries_GivenNoScriptWithId_ShouldReturnScriptDoesNotExistsError()
    {
        // Arrange
        var projectId = ProjectId.FromValue(Guid.NewGuid().ToString());
        var scriptId = new ScriptId(Guid.NewGuid().ToString());

        // Act
        var result = await _adapter.ListEntries(
            projectId,
            scriptId,
            0,
            1,
            TestContext.Current.CancellationToken
        );

        // Assert
        result.ShouldBe(new ScriptDoesNotExistsError(scriptId));
    }

    [Fact]
    public async Task ListEntries_GivenScript_ShouldReturnOneElement()
    {
        // Arrange
        var projectId = ProjectId.FromValue(Guid.NewGuid().ToString());
        var scriptId = new ScriptId(Guid.NewGuid().ToString());
        await _mongoCollection.InsertOneAsync(
            CreateMongodbScript(scriptId.ToString(), [], "path/file.txt", projectId.Value),
            cancellationToken: TestContext.Current.CancellationToken
        );

        // Act
        var result = await _adapter.ListEntries(
            projectId,
            scriptId,
            0,
            1,
            TestContext.Current.CancellationToken
        );

        // Assert
        result.IsT0.ShouldBeTrue();
        var page = result.AsT0;
        page.Offset.ShouldBe(0);
        page.Limit.ShouldBe(1);
        page.TotalCount.ShouldBe(1);
        var entries = page.Data.ToList();
        entries.Count.ShouldBe(1);
        entries[0]
            .AssertProjectStructureEntry(
                CreateScript(scriptId, [], "path/file.txt", projectId.ToString())
            );
    }

    [Fact]
    public async Task ListEntries_GivenMultipleEntries_ShouldReturnOnlyChildEntriesOfFolder()
    {
        // Arrange
        var projectId = ProjectId.FromValue(Guid.NewGuid().ToString());
        var scriptIdToSearch = new ScriptId(Guid.NewGuid().ToString());
        var scriptId1 = Guid.NewGuid().ToString();
        var scriptId2 = Guid.NewGuid().ToString();

        await _mongoCollection.InsertOneAsync(
            CreateMongodbScriptFolder(scriptIdToSearch.ToString(), [scriptId1, scriptId2], "root"),
            cancellationToken: TestContext.Current.CancellationToken
        );

        await _mongoCollection.InsertOneAsync(
            CreateMongodbScript(Guid.NewGuid().ToString(), [], "path/file.txt", projectId.Value),
            cancellationToken: TestContext.Current.CancellationToken
        );
        await _mongoCollection.InsertOneAsync(
            CreateMongodbScript(scriptId1, [], "root/file.txt", projectId.Value),
            cancellationToken: TestContext.Current.CancellationToken
        );
        await _mongoCollection.InsertOneAsync(
            CreateMongodbScriptFolder(scriptId2, [], "root/folder", projectId.Value),
            cancellationToken: TestContext.Current.CancellationToken
        );
        await _mongoCollection.InsertOneAsync(
            CreateMongodbScript(Guid.NewGuid().ToString(), [], "root"),
            cancellationToken: TestContext.Current.CancellationToken
        );

        // Act
        var result = await _adapter.ListEntries(
            projectId,
            scriptIdToSearch,
            0,
            4,
            TestContext.Current.CancellationToken
        );

        // Assert
        result.IsT0.ShouldBeTrue();
        var page = result.AsT0;
        page.Offset.ShouldBe(0);
        page.Limit.ShouldBe(4);
        page.TotalCount.ShouldBe(2);
        var entries = page.Data.ToList();
        entries.Count.ShouldBe(2);
        entries
            .Find(e => e.Id.ToString() == scriptId1)!
            .AssertProjectStructureEntry(
                CreateScript(new ScriptId(scriptId1), [], "root/file.txt", projectId.ToString())
            );
        entries
            .Find(e => e.Id.ToString() == scriptId2)!
            .AssertProjectStructureEntry(
                CreateScriptFolder(new ScriptId(scriptId2), "root/folder", projectId.ToString(), [])
            );
    }

    [Fact]
    public async Task ListEntries_GivenMultipleEntries_ShouldReturnOnlyEntriesFromFolderWithinLimitAndOffset()
    {
        // Arrange
        var projectId = ProjectId.FromValue(Guid.NewGuid().ToString());
        var scriptIdToSearch = new ScriptId(Guid.NewGuid().ToString());
        var scriptId1 = Guid.NewGuid().ToString();
        var scriptId2 = Guid.NewGuid().ToString();
        var scriptId3 = Guid.NewGuid().ToString();

        await _mongoCollection.InsertOneAsync(
            CreateMongodbScriptFolder(
                scriptIdToSearch.ToString(),
                [scriptId1, scriptId2, scriptId3],
                "root/folder"
            ),
            cancellationToken: TestContext.Current.CancellationToken
        );

        await _mongoCollection.InsertOneAsync(
            CreateMongodbScript(Guid.NewGuid().ToString(), [], "path/file.txt", projectId.Value),
            cancellationToken: TestContext.Current.CancellationToken
        );
        await _mongoCollection.InsertOneAsync(
            CreateMongodbScript(scriptId1, [], "root/folder/file.txt", projectId.Value),
            cancellationToken: TestContext.Current.CancellationToken
        );
        await _mongoCollection.InsertOneAsync(
            CreateMongodbScriptFolder(scriptId2, [], "root/folder/subfolder", projectId.Value),
            cancellationToken: TestContext.Current.CancellationToken
        );
        await _mongoCollection.InsertOneAsync(
            CreateMongodbScript(scriptId3, [], "root/folder/file-2.txt", projectId.Value),
            cancellationToken: TestContext.Current.CancellationToken
        );
        await _mongoCollection.InsertOneAsync(
            CreateMongodbScript(Guid.NewGuid().ToString(), [], "root"),
            cancellationToken: TestContext.Current.CancellationToken
        );

        // Act
        var result = await _adapter.ListEntries(
            projectId,
            scriptIdToSearch,
            1,
            1,
            TestContext.Current.CancellationToken
        );

        // Assert
        result.IsT0.ShouldBeTrue();
        var page = result.AsT0;
        page.Offset.ShouldBe(1);
        page.Limit.ShouldBe(1);
        page.TotalCount.ShouldBe(3);
        var entries = page.Data.ToList();
        entries.Count.ShouldBe(1);
        entries[0]
            .AssertProjectStructureEntry(
                CreateScriptFolder(
                    new ScriptId(scriptId2),
                    "root/folder/subfolder",
                    projectId.ToString(),
                    []
                )
            );
    }
}

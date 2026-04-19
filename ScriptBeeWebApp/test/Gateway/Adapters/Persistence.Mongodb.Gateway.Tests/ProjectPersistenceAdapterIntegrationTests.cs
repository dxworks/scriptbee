using MongoDB.Driver;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.File;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Persistence.Mongodb.Entity;
using ScriptBee.Persistence.Mongodb.Repository;
using ScriptBee.Tests.Common;
using ScriptBee.Tests.Common.Mongodb;

namespace ScriptBee.Persistence.Mongodb.Tests;

public class ProjectPersistenceAdapterIntegrationTests : IClassFixture<MongoDbFixture>
{
    private readonly ProjectPersistenceAdapter _adapter;
    private readonly IMongoCollection<MongodbProjectModel> _mongoCollection;

    public ProjectPersistenceAdapterIntegrationTests(
        MongoDbFixture fixture,
        ITestOutputHelper outputHelper
    )
    {
        _mongoCollection = fixture.GetCollection<MongodbProjectModel>("Projects");
        _adapter = new ProjectPersistenceAdapter(
            new MongoRepository<MongodbProjectModel>(_mongoCollection),
            new XUnitLogger<ProjectPersistenceAdapter>(outputHelper)
        );
    }

    [Fact]
    public async Task CreateNewProject()
    {
        var project = ProjectDetailsFixture.BasicProjectDetails(ProjectId.Create("id"));

        var result = await _adapter.Create(project, TestContext.Current.CancellationToken);

        result.IsT0.ShouldBeTrue();
        var savedProject = await _mongoCollection
            .Find(p => p.Id == "id")
            .FirstOrDefaultAsync(cancellationToken: TestContext.Current.CancellationToken);
        savedProject.Id.ShouldBe("id");
        savedProject.Name.ShouldBe("project");
        savedProject.CreationDate.ShouldBe(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task GivenIdAlreadyExists_CreateProject_ExpectProjectAlreadyExistsError()
    {
        var projectId = ProjectId.Create("existing-id");
        var project = ProjectDetailsFixture.BasicProjectDetails(projectId);
        await _mongoCollection.InsertOneAsync(
            new MongodbProjectModel { Id = "existing-id", Name = "existing" },
            cancellationToken: TestContext.Current.CancellationToken
        );

        var result = await _adapter.Create(project, TestContext.Current.CancellationToken);

        result.AsT1.ShouldBe(new ProjectIdAlreadyInUseError(projectId));
    }

    [Fact]
    public async Task DeleteProject()
    {
        var projectId = ProjectId.FromValue("to-delete");
        await _mongoCollection.InsertOneAsync(
            new MongodbProjectModel { Id = "to-delete", Name = "to-delete" },
            cancellationToken: TestContext.Current.CancellationToken
        );

        await _adapter.Delete(projectId, TestContext.Current.CancellationToken);

        var project = await _mongoCollection
            .Find(p => p.Id == "to-delete")
            .FirstOrDefaultAsync(cancellationToken: TestContext.Current.CancellationToken);
        project.ShouldBeNull();
    }

    [Fact]
    public async Task GivenNotExistingId_DeleteProject_ExpectNoError()
    {
        var projectId = ProjectId.Create("to-delete-not-existing-id");

        await Should.NotThrowAsync(async () =>
            await _adapter.Delete(projectId, TestContext.Current.CancellationToken)
        );
    }

    [Fact]
    public async Task GetAllProjects()
    {
        var creationDate = DateTimeOffset.UtcNow;
        await _mongoCollection.InsertOneAsync(
            new MongodbProjectModel
            {
                Id = "all-projects-id",
                Name = "all-projects-id",
                CreationDate = creationDate,
            },
            cancellationToken: TestContext.Current.CancellationToken
        );

        var projectDetailsList = await _adapter.GetAll(TestContext.Current.CancellationToken);

        var projectId = ProjectId.FromValue("all-projects-id");
        var projectDetails = projectDetailsList.First(x => x.Id == projectId);
        projectDetails.Id.ShouldBe(ProjectId.FromValue("all-projects-id"));
        projectDetails.Name.ShouldBe("all-projects-id");
        projectDetails.CreationDate.ShouldBe(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task GetProjectByIdWithAllProperties()
    {
        var creationDate = DateTimeOffset.UtcNow;
        const string projectId = "get-project-by-id";
        await _mongoCollection.InsertOneAsync(
            new MongodbProjectModel
            {
                Id = projectId,
                Name = projectId,
                CreationDate = creationDate,
                SavedFiles =
                {
                    {
                        "loader-id",
                        [new MongodbFileData("957969a8-c66e-498d-b00f-7e58ded36b80", "file")]
                    },
                },
                LoadedFiles =
                {
                    {
                        "loader-id",
                        [new MongodbFileData("1209bb17-e623-454b-a441-0102fa64daf6", "file")]
                    },
                },
                Linkers = ["linker-id"],
                InstalledPlugins =
                [
                    new MongodbPluginInstallationConfig
                    {
                        PluginId = "plugin-id",
                        Version = "1.2.3",
                    },
                ],
            },
            cancellationToken: TestContext.Current.CancellationToken
        );

        var result = await _adapter.GetById(
            ProjectId.Create(projectId),
            TestContext.Current.CancellationToken
        );

        result.AsT0.Id.ShouldBe(ProjectId.FromValue(projectId));
        result.AsT0.Name.ShouldBe(projectId);
        result.AsT0.CreationDate.ShouldBe(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));
        var savedFile = result.AsT0.SavedFiles.Single();
        savedFile.Key.ShouldBe("loader-id");
        savedFile
            .Value.Single()
            .ShouldBe(new FileData(new FileId("957969a8-c66e-498d-b00f-7e58ded36b80"), "file"));
        var loadedFile = result.AsT0.LoadedFiles.Single();
        loadedFile.Key.ShouldBe("loader-id");
        loadedFile
            .Value.Single()
            .ShouldBe(new FileData(new FileId("1209bb17-e623-454b-a441-0102fa64daf6"), "file"));
        result.AsT0.Linkers.Single().ShouldBe("linker-id");
        result
            .AsT0.InstalledPlugins.Single()
            .ShouldBeEquivalentTo(new PluginInstallationConfig("plugin-id", new Version("1.2.3")));
    }

    [Fact]
    public async Task GetProjectByIdWithMinimalProperties()
    {
        var creationDate = DateTimeOffset.UtcNow;
        const string projectId = "get-project-by-id-with-minimal-properties";
        await _mongoCollection.InsertOneAsync(
            new MongodbProjectModel
            {
                Id = projectId,
                Name = projectId,
                CreationDate = creationDate,
            },
            cancellationToken: TestContext.Current.CancellationToken
        );

        var result = await _adapter.GetById(
            ProjectId.Create(projectId),
            TestContext.Current.CancellationToken
        );

        result.AsT0.Id.ShouldBe(ProjectId.FromValue(projectId));
        result.AsT0.Name.ShouldBe(projectId);
        result.AsT0.CreationDate.ShouldBe(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(2));
        result.AsT0.SavedFiles.ShouldBeEmpty();
        result.AsT0.LoadedFiles.ShouldBeEmpty();
        result.AsT0.Linkers.ShouldBeEmpty();
        result.AsT0.InstalledPlugins.ShouldBeEmpty();
    }

    [Fact]
    public async Task GivenNoProject_GetProjectById_ShouldReturnProjectDoesNotExistsError()
    {
        var result = await _adapter.GetById(
            ProjectId.Create("get-project-by-not-existing-id"),
            TestContext.Current.CancellationToken
        );

        result.ShouldBe(
            new ProjectDoesNotExistsError(ProjectId.FromValue("get-project-by-not-existing-id"))
        );
    }

    [Fact]
    public async Task UpdateProject()
    {
        var creationDate = DateTimeOffset.UtcNow;
        await _mongoCollection.InsertOneAsync(
            new MongodbProjectModel
            {
                Id = "update-project-id",
                Name = "update-projects-id",
                CreationDate = creationDate,
            },
            cancellationToken: TestContext.Current.CancellationToken
        );
        var projectId = ProjectId.FromValue("update-project-id");
        var project = new ProjectDetails(
            projectId,
            "updated-name",
            DateTimeOffset.Parse("2024-02-08"),
            new Dictionary<string, List<FileData>>
            {
                {
                    "loader-id",
                    [new FileData(new FileId("7609bb17-e623-454b-a441-0102fa64daf6"), "file")]
                },
            },
            new Dictionary<string, List<FileData>>
            {
                {
                    "loader-id",
                    [new FileData(new FileId("1209bb17-e623-454b-a441-0102fa64daf6"), "file")]
                },
            },
            ["linker-id"],
            [new PluginInstallationConfig("plugin-id", new Version("1.2.3"))]
        );

        var updateProject = await _adapter.Update(project, TestContext.Current.CancellationToken);

        updateProject.ShouldBe(project);
        var updatedMongoProject = await _mongoCollection
            .Find(p => p.Id == "update-project-id")
            .FirstOrDefaultAsync(cancellationToken: TestContext.Current.CancellationToken);
        updatedMongoProject.Id.ShouldBe(projectId.Value);
        updatedMongoProject.Name.ShouldBe("updated-name");
        updatedMongoProject.CreationDate.ShouldBe(DateTimeOffset.Parse("2024-02-08"));
        var loaderPair = updatedMongoProject.SavedFiles.Single();
        loaderPair.Key.ShouldBe("loader-id");
        loaderPair
            .Value.Single()
            .ShouldBe(new MongodbFileData("7609bb17-e623-454b-a441-0102fa64daf6", "file"));
        var linkerPair = updatedMongoProject.LoadedFiles.Single();
        linkerPair.Key.ShouldBe("loader-id");
        linkerPair
            .Value.Single()
            .ShouldBe(new MongodbFileData("1209bb17-e623-454b-a441-0102fa64daf6", "file"));
        updatedMongoProject.Linkers.Single().ShouldBe("linker-id");
        updatedMongoProject
            .InstalledPlugins.Single()
            .ShouldBeEquivalentTo(
                new MongodbPluginInstallationConfig { PluginId = "plugin-id", Version = "1.2.3" }
            );
    }
}

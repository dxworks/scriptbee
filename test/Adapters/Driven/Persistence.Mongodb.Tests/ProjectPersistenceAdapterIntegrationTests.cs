using MongoDB.Driver;
using ScriptBee.Domain.Model.File;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Persistence.Mongodb.Entity;
using ScriptBee.Persistence.Mongodb.Repository;
using ScriptBee.Tests.Common;
using Xunit.Abstractions;
using static ScriptBee.Tests.Common.ProjectDetailsFixture;

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
            new XunitLogger<ProjectPersistenceAdapter>(outputHelper)
        );
    }

    [Fact]
    public async Task CreateNewProject()
    {
        var project = BasicProjectDetails(ProjectId.Create("id"));

        var result = await _adapter.Create(project, CancellationToken.None);

        result.IsT0.ShouldBeTrue();
        var savedProject = await _mongoCollection.Find(p => p.Id == "id").FirstOrDefaultAsync();
        savedProject.Id.ShouldBe("id");
        savedProject.Name.ShouldBe("project");
        savedProject.CreationDate.ShouldBe(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task GivenIdAlreadyExists_CreateProject_ExpectProjectAlreadyExistsError()
    {
        var projectId = ProjectId.Create("existing-id");
        var project = BasicProjectDetails(projectId);
        await _mongoCollection.InsertOneAsync(
            new MongodbProjectModel { Id = "existing-id", Name = "existing" }
        );

        var result = await _adapter.Create(project, CancellationToken.None);

        result.AsT1.ShouldBe(new ProjectIdAlreadyInUseError(projectId));
    }

    [Fact]
    public async Task DeleteProject()
    {
        var projectId = ProjectId.FromValue("to-delete");
        await _mongoCollection.InsertOneAsync(
            new MongodbProjectModel { Id = "to-delete", Name = "to-delete" }
        );

        await _adapter.Delete(projectId, CancellationToken.None);

        var project = await _mongoCollection.Find(p => p.Id == "to-delete").FirstOrDefaultAsync();
        project.ShouldBeNull();
    }

    [Fact]
    public async Task GivenNotExistingId_DeleteProject_ExpectNoError()
    {
        var projectId = ProjectId.Create("to-delete-not-existing-id");

        await Should.NotThrowAsync(
            async () => await _adapter.Delete(projectId, CancellationToken.None)
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
            }
        );

        var projectDetailsList = await _adapter.GetAll(CancellationToken.None);

        var projectId = ProjectId.FromValue("all-projects-id");
        var projectDetails = projectDetailsList.First(x => x.Id == projectId);
        projectDetails.Id.ShouldBe(ProjectId.FromValue("all-projects-id"));
        projectDetails.Name.ShouldBe("all-projects-id");
        projectDetails.CreationDate.ShouldBe(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task GetProjectById()
    {
        var creationDate = DateTimeOffset.UtcNow;
        await _mongoCollection.InsertOneAsync(
            new MongodbProjectModel
            {
                Id = "get-project-by-id",
                Name = "get-project-by-id",
                CreationDate = creationDate,
                SavedFiles =
                {
                    {
                        "loader-id",
                        [new MongodbFileData("957969a8-c66e-498d-b00f-7e58ded36b80", "file")]
                    },
                },
            }
        );

        var result = await _adapter.GetById(
            ProjectId.Create("get-project-by-id"),
            CancellationToken.None
        );

        result.AsT0.Id.ShouldBe(ProjectId.FromValue("get-project-by-id"));
        result.AsT0.Name.ShouldBe("get-project-by-id");
        result.AsT0.CreationDate.ShouldBe(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));
        var keyValuePair = result.AsT0.SavedFiles.Single();
        keyValuePair.Key.ShouldBe("loader-id");
        keyValuePair
            .Value.Single()
            .ShouldBe(new FileData(new FileId("957969a8-c66e-498d-b00f-7e58ded36b80"), "file"));
    }

    [Fact]
    public async Task GivenNoProject_GetProjectById_ShouldReturnProjectDoesNotExistsError()
    {
        var result = await _adapter.GetById(
            ProjectId.Create("get-project-by-not-existing-id"),
            CancellationToken.None
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
            }
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
            ["linker-id"]
        );

        var updateProject = await _adapter.Update(project, CancellationToken.None);

        updateProject.ShouldBe(project);
        var updatedMongoProject = await _mongoCollection
            .Find(p => p.Id == "update-project-id")
            .FirstOrDefaultAsync();
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
    }
}

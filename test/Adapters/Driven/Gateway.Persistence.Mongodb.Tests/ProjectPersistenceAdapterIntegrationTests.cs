using MongoDB.Driver;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Gateway.Persistence.Mongodb.Contracts;
using ScriptBee.Gateway.Persistence.Mongodb.Repository;
using ScriptBee.Tests.Common;
using Shouldly;
using Xunit.Abstractions;

namespace ScriptBee.Gateway.Persistence.Mongodb.Tests;

public class ProjectPersistenceAdapterIntegrationTests : IClassFixture<MongoDbFixture>
{
    private readonly ProjectPersistenceAdapter _adapter;
    private readonly IMongoCollection<ProjectModel> _mongoCollection;

    public ProjectPersistenceAdapterIntegrationTests(MongoDbFixture fixture, ITestOutputHelper outputHelper)
    {
        _mongoCollection = fixture.GetCollection<ProjectModel>("Projects");
        _adapter = new ProjectPersistenceAdapter(new MongoRepository<ProjectModel>(_mongoCollection),
            new XunitLogger(outputHelper));
    }

    [Fact]
    public async Task CreateNewProject()
    {
        var project = new ProjectDetails(ProjectId.Create("id"), "name", DateTimeOffset.UtcNow);

        var result = await _adapter.Create(project, CancellationToken.None);

        result.IsT0.ShouldBeTrue();
        var savedProject = await _mongoCollection.Find(p => p.Id == "id").FirstOrDefaultAsync();
        savedProject.Id.ShouldBe("id");
        savedProject.Name.ShouldBe("name");
        savedProject.CreationDate.ShouldBe(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task GivenIdAlreadyExists_CreateProject_ExpectProjectAlreadyExistsError()
    {
        var projectId = ProjectId.Create("existing-id");
        var project = new ProjectDetails(projectId, "name", DateTimeOffset.UtcNow);
        await _mongoCollection.InsertOneAsync(new ProjectModel { Id = "existing-id", Name = "existing" });

        var result = await _adapter.Create(project, CancellationToken.None);

        result.AsT1.ShouldBe(new ProjectIdAlreadyInUseError(projectId));
    }

    [Fact]
    public async Task DeleteProject()
    {
        var projectId = ProjectId.FromValue("to-delete");
        await _mongoCollection.InsertOneAsync(new ProjectModel { Id = "to-delete", Name = "to-delete" });

        await _adapter.Delete(projectId, CancellationToken.None);

        var project = await _mongoCollection.Find(p => p.Id == "to-delete").FirstOrDefaultAsync();
        project.ShouldBeNull();
    }

    [Fact]
    public async Task GivenNotExistingId_DeleteProject_ExpectNoError()
    {
        var projectId = ProjectId.Create("to-delete-not-existing-id");

        await Should.NotThrowAsync(async () => await _adapter.Delete(projectId, CancellationToken.None));
    }

    [Fact]
    public async Task GetAllProjects()
    {
        var creationDate = DateTimeOffset.UtcNow;
        await _mongoCollection.InsertOneAsync(new ProjectModel
            { Id = "all-projects-id", Name = "all-projects-id", CreationDate = creationDate });

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
        await _mongoCollection.InsertOneAsync(new ProjectModel
            { Id = "get-project-by-id", Name = "get-project-by-id", CreationDate = creationDate });

        var result = await _adapter.GetById(ProjectId.Create("get-project-by-id"), CancellationToken.None);

        result.AsT0.Id.ShouldBe(ProjectId.FromValue("get-project-by-id"));
        result.AsT0.Name.ShouldBe("get-project-by-id");
        result.AsT0.CreationDate.ShouldBe(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task GivenNoProject_GetProjectById_ShouldReturnProjectDoesNotExistsError()
    {
        var result = await _adapter.GetById(ProjectId.Create("get-project-by-not-existing-id"), CancellationToken.None);

        result.ShouldBe(
            new ProjectDoesNotExistsError(ProjectId.FromValue("get-project-by-not-existing-id"))
        );
    }
}

using MongoDB.Driver;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Gateway.Persistence.Mongodb.Contracts;
using ScriptBee.Gateway.Persistence.Mongodb.Repository;
using Shouldly;

namespace ScriptBee.Gateway.Persistence.Mongodb.Tests;

public class ProjectPersistenceAdapterIntegrationTests : IClassFixture<MongoDbFixture>
{
    private readonly ProjectPersistenceAdapter _adapter;
    private readonly IMongoCollection<ProjectModel> _mongoCollection;

    public ProjectPersistenceAdapterIntegrationTests(MongoDbFixture fixture)
    {
        _mongoCollection = fixture.GetCollection<ProjectModel>("Projects");
        _adapter = new ProjectPersistenceAdapter(new MongoRepository<ProjectModel>(_mongoCollection), new TestLogger());
    }

    [Fact]
    public async Task CreateNewProject()
    {
        var project = new ProjectDetails(ProjectId.Create("id"), "name");

        var result = await _adapter.CreateProject(project, CancellationToken.None);

        result.IsT0.ShouldBeTrue();
        var savedProject = await _mongoCollection.Find(p => p.Id == "id").FirstOrDefaultAsync();
        savedProject.Id.ShouldBe("id");
        savedProject.Name.ShouldBe("name");
        savedProject.CreationDate.ShouldBe(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task GivenIdAlreadyExists_CreateProject_ExpectProjectAlreadyExistsError()
    {
        var projectId = ProjectId.Create("existing-id");
        var project = new ProjectDetails(projectId, "name");
        await _mongoCollection.InsertOneAsync(new ProjectModel { Id = "existing-id", Name = "existing" });

        var result = await _adapter.CreateProject(project, CancellationToken.None);

        result.AsT1.ShouldBe(new ProjectIdAlreadyInUseError(projectId));
    }

    [Fact]
    public async Task DeleteProject()
    {
        var projectId = ProjectId.FromValue("to-delete");
        await _mongoCollection.InsertOneAsync(new ProjectModel { Id = "to-delete", Name = "to-delete" });

        await _adapter.DeleteProject(projectId, CancellationToken.None);

        var project = await _mongoCollection.Find(p => p.Id == "to-delete").FirstOrDefaultAsync();
        project.ShouldBeNull();
    }

    [Fact]
    public async Task GivenNotExistingId_DeleteProject_ExpectNoError()
    {
        var projectId = ProjectId.Create("to-delete-not-existing-id");

        await Should.NotThrowAsync(async () => await _adapter.DeleteProject(projectId, CancellationToken.None));
    }
}

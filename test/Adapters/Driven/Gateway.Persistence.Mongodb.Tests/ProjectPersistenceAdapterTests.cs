using System.Linq.Expressions;
using NSubstitute;
using ScriptBee.Domain.Model.Projects;
using ScriptBee.Gateway.Persistence.Mongodb.Contracts;
using ScriptBee.Gateway.Persistence.Mongodb.Repository;

namespace ScriptBee.Gateway.Persistence.Mongodb.Tests;

public class ProjectPersistenceAdapterTests
{
    private readonly IMongoRepository<ProjectModel> _mongoRepository = Substitute.For<IMongoRepository<ProjectModel>>();

    [Fact]
    public async Task CreateProject()
    {
        var project = new Project(ProjectId.FromValue("id"), "name");

        var adapter = new ProjectPersistenceAdapter(_mongoRepository);
        await adapter.CreateProject(project, CancellationToken.None);

        await _mongoRepository.Received(1)
            .CreateDocument(Arg.Is(CreateProjectModelPredicate()
            ), Arg.Any<CancellationToken>());
    }

    private static Expression<Predicate<ProjectModel>> CreateProjectModelPredicate()
    {
        return projectModel =>
            projectModel.Id == "id" &&
            projectModel.Name == "name" &&
            Math.Abs((projectModel.CreationDate - DateTime.UtcNow).TotalSeconds) < 5;
    }
}

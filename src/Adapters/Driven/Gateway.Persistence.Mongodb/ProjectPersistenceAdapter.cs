using ScriptBee.Domain.Model.Projects;
using ScriptBee.Gateway.Persistence.Mongodb.Contracts;
using ScriptBee.Gateway.Persistence.Mongodb.Repository;
using ScriptBee.Ports.Driven.Projects;

namespace ScriptBee.Gateway.Persistence.Mongodb;

public class ProjectPersistenceAdapter(IMongoRepository<ProjectModel> mongoRepository) : ICreateProject
{
    public async Task CreateProject(Project project, CancellationToken cancellationToken = default)
    {
        await mongoRepository.CreateDocument(ProjectModel.From(project, DateTime.UtcNow), cancellationToken);
    }
}

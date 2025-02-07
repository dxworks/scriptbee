using MongoDB.Driver;
using OneOf;
using ScriptBee.Domain.Model;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Gateway.Persistence.Mongodb.Contracts;
using ScriptBee.Gateway.Persistence.Mongodb.Repository;
using ScriptBee.Ports.Driven.Projects;
using ILogger = Serilog.ILogger;

namespace ScriptBee.Gateway.Persistence.Mongodb;

public class ProjectPersistenceAdapter(IMongoRepository<ProjectModel> mongoRepository, ILogger logger) : ICreateProject
{
    public async Task<OneOf<Unit, ProjectIdAlreadyInUseError>> CreateProject(ProjectDetails projectDetails,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await mongoRepository.CreateDocument(ProjectModel.From(projectDetails, DateTime.UtcNow), cancellationToken);
        }
        catch (MongoWriteException e)
        {
            logger.Error(e, "Project with id {Id} already exists", projectDetails.Id);
            return new ProjectIdAlreadyInUseError(projectDetails.Id);
        }

        return new Unit();
    }
}

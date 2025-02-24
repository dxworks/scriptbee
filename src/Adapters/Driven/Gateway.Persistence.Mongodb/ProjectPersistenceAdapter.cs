using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using OneOf;
using ScriptBee.Domain.Model;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Gateway.Persistence.Mongodb.Contracts;
using ScriptBee.Gateway.Persistence.Mongodb.Repository;
using ScriptBee.Ports.Driven.Project;

namespace ScriptBee.Gateway.Persistence.Mongodb;

public class ProjectPersistenceAdapter(IMongoRepository<ProjectModel> mongoRepository, ILogger logger)
    : ICreateProject, IDeleteProject, IGetAllProjects, IGetProject
{
    public async Task<OneOf<Unit, ProjectIdAlreadyInUseError>> Create(ProjectDetails projectDetails,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await mongoRepository.CreateDocument(ProjectModel.From(projectDetails), cancellationToken);
        }
        catch (MongoWriteException e)
        {
            logger.LogError(e, "Project with id {Id} already exists", projectDetails.Id);
            return new ProjectIdAlreadyInUseError(projectDetails.Id);
        }

        return new Unit();
    }

    public async Task Delete(ProjectId projectId, CancellationToken cancellationToken = default)
    {
        await mongoRepository.DeleteDocument(projectId.Value, cancellationToken);
    }

    public async Task<IEnumerable<ProjectDetails>> GetAll(CancellationToken cancellationToken = default)
    {
        var projectModels = await mongoRepository.GetAllDocuments(cancellationToken);
        return projectModels.Select(model => model.ToProjectDetails());
    }

    public async Task<OneOf<ProjectDetails, ProjectDoesNotExistsError>> GetById(ProjectId projectId,
        CancellationToken cancellationToken = default)
    {
        var projectModel = await mongoRepository.GetDocument(projectId.Value, cancellationToken);

        if (projectModel == null)
        {
            return new ProjectDoesNotExistsError(projectId);
        }

        return projectModel.ToProjectDetails();
    }
}

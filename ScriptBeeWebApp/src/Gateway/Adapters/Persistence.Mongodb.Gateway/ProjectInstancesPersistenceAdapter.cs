using OneOf;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Persistence.Mongodb.Entity;
using ScriptBee.Persistence.Mongodb.Repository;
using ScriptBee.Ports.Instance;

namespace ScriptBee.Persistence.Mongodb;

public sealed class ProjectInstancesPersistenceAdapter(
    IMongoRepository<MongodbProjectInstance> mongoRepository
) : ICreateProjectInstance, IGetAllProjectInstances, IGetProjectInstance, IDeleteProjectInstance
{
    public async Task<InstanceInfo> Create(
        InstanceInfo instanceInfo,
        CancellationToken cancellationToken
    )
    {
        var projectInstance = MongodbProjectInstance.From(instanceInfo);

        await mongoRepository.CreateDocument(projectInstance, cancellationToken);

        return instanceInfo;
    }

    public async Task<IEnumerable<InstanceInfo>> GetAll(
        ProjectId projectId,
        CancellationToken cancellationToken
    )
    {
        var projectInstances = await mongoRepository.GetAllDocuments(
            instance => instance.ProjectId == projectId.Value,
            cancellationToken
        );

        return projectInstances.Select(instance => instance.ToCalculationInstanceInfo());
    }

    public async Task<OneOf<InstanceInfo, InstanceDoesNotExistsError>> Get(
        InstanceId id,
        CancellationToken cancellationToken
    )
    {
        var instance = await mongoRepository.GetDocument(id.ToString(), cancellationToken);

        if (instance is null)
        {
            return new InstanceDoesNotExistsError(id);
        }

        return instance.ToCalculationInstanceInfo();
    }

    public async Task Delete(InstanceInfo instanceInfo, CancellationToken cancellationToken)
    {
        await mongoRepository.DeleteDocument(instanceInfo.Id.ToString(), cancellationToken);
    }
}

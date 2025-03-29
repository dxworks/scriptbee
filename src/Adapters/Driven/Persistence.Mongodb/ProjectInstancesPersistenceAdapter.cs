using OneOf;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Persistence.Mongodb.Entity;
using ScriptBee.Persistence.Mongodb.Repository;
using ScriptBee.Ports.Instance;

namespace ScriptBee.Persistence.Mongodb;

public class ProjectInstancesPersistenceAdapter(
    IMongoRepository<MongodbProjectInstance> mongoRepository
) : ICreateProjectInstance, IGetAllProjectInstances, IGetProjectInstance
{
    public async Task<InstanceInfo> Create(
        ProjectId projectId,
        CancellationToken cancellationToken = default
    )
    {
        // TODO FIXIT(#63, #45): implement this without hardcoded values
        var calculationInstanceInfo = new InstanceInfo(
            new InstanceId("test"),
            projectId,
            "http://localhost:5002",
            DateTimeOffset.UtcNow
        );
        var projectInstance = MongodbProjectInstance.From(calculationInstanceInfo);

        await mongoRepository.CreateDocument(projectInstance, cancellationToken);

        return calculationInstanceInfo;
    }

    public async Task<IEnumerable<InstanceInfo>> GetAll(
        ProjectId projectId,
        CancellationToken cancellationToken = default
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
        CancellationToken cancellationToken = default
    )
    {
        var instance = await mongoRepository.GetDocument(id.ToString(), cancellationToken);

        if (instance is null)
        {
            return new InstanceDoesNotExistsError(id);
        }

        return instance.ToCalculationInstanceInfo();
    }
}

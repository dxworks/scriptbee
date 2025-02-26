using ScriptBee.Analysis.Ports;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Persistence.Mongodb.Contracts;
using ScriptBee.Persistence.Mongodb.Repository;

namespace ScriptBee.Persistence.Mongodb;

public class ProjectInstancesPersistenceAdapter(IMongoRepository<ProjectInstance> mongoRepository)
    : ICreateProjectInstance,
        IGetAllProjectInstances
{
    public async Task<InstanceInfo> Create(
        ProjectId projectId,
        CancellationToken cancellationToken = default
    )
    {
        var calculationInstanceInfo = new InstanceInfo(
            InstanceId.FromValue("test"),
            projectId,
            "http://localhost:5002",
            DateTimeOffset.UtcNow
        );
        var projectInstance = ProjectInstance.From(calculationInstanceInfo);

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
}

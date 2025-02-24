using ScriptBee.Domain.Model.Calculation;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Gateway.Persistence.Mongodb.Contracts;
using ScriptBee.Gateway.Persistence.Mongodb.Repository;
using ScriptBee.Ports.Driven.Calculation;

namespace ScriptBee.Gateway.Persistence.Mongodb;

public class ProjectInstancesPersistenceAdapter(IMongoRepository<ProjectInstance> mongoRepository)
    : IGetAllProjectInstances
{
    public async Task<IEnumerable<CalculationInstanceInfo>> GetAll(ProjectId projectId,
        CancellationToken cancellationToken = default)
    {
        var projectInstances = await mongoRepository.GetAllDocuments(instance => instance.ProjectId == projectId.Value,
            cancellationToken);

        return projectInstances.Select(instance => instance.ToCalculationInstanceInfo());
    }
}

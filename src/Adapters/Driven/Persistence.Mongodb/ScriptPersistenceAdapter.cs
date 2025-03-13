using ScriptBee.Domain.Model.ProjectStructure;
using ScriptBee.Persistence.Mongodb.Entity.Script;
using ScriptBee.Persistence.Mongodb.Repository;
using ScriptBee.Ports.Project.Structure;

namespace ScriptBee.Persistence.Mongodb;

public class ScriptPersistenceAdapter(IMongoRepository<MongodbScript> mongoRepository)
    : ICreateScript
{
    public async Task Create(Script script, CancellationToken cancellationToken = default)
    {
        await mongoRepository.CreateDocument(MongodbScript.From(script), cancellationToken);
    }
}

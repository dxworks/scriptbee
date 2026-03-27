using OneOf;
using ScriptBee.Artifacts.Mongodb.Entity.Script;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.ProjectStructure;
using ScriptBee.Persistence.Mongodb.Repository;

namespace ScriptBee.Artifacts.Mongodb;

public class ScriptPersistenceAdapter(IMongoRepository<MongodbScript> mongoRepository)
    : ICreateScript,
        IGetScript
{
    public async Task Create(Script script, CancellationToken cancellationToken = default)
    {
        await mongoRepository.CreateDocument(MongodbScript.From(script), cancellationToken);
    }

    public async Task<OneOf<Script, ScriptDoesNotExistsError>> Get(
        ScriptId scriptId,
        CancellationToken cancellationToken = default
    )
    {
        var mongodbScript = await mongoRepository.GetDocument(
            scriptId.ToString(),
            cancellationToken
        );

        if (mongodbScript == null)
        {
            return new ScriptDoesNotExistsError(scriptId);
        }

        return mongodbScript.ToScript();
    }
}

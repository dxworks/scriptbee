using OneOf;
using ScriptBee.Artifacts.Mongodb.Entity.Script;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.ProjectStructure;
using ScriptBee.Persistence.Mongodb.Repository;

namespace ScriptBee.Artifacts.Mongodb;

public class ScriptsPersistenceAdapter(IMongoRepository<MongodbScript> mongoRepository)
    : ICreateScript,
        IGetScripts
{
    public async Task Create(Script script, CancellationToken cancellationToken = default)
    {
        await mongoRepository.CreateDocument(MongodbScript.From(script), cancellationToken);
    }

    public async Task<IEnumerable<Script>> GetAll(
        ProjectId projectId,
        CancellationToken cancellationToken
    )
    {
        var scripts = await mongoRepository.GetAllDocuments(
            s => s.ProjectId == projectId.ToString(),
            cancellationToken
        );

        return scripts.Select(s => s.ToScript());
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

using OneOf;
using ScriptBee.Artifacts.Mongodb.Entity.Script;
using ScriptBee.Common;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.ProjectStructure;
using ScriptBee.Persistence.Mongodb.Repository;

namespace ScriptBee.Artifacts.Mongodb;

public class ScriptsPersistenceAdapter(
    IMongoRepository<MongodbScript> mongoRepository,
    IGuidProvider guidProvider
) : ICreateScript, IGetScripts, IUpdateScript
{
    private const int MaxDepth = 10_000;

    public async Task Create(Script script, CancellationToken cancellationToken)
    {
        var mongodbScript = MongodbScript.From(script);
        await mongoRepository.CreateDocument(mongodbScript, cancellationToken);

        await CreateParentFolder(mongodbScript, cancellationToken);
    }

    public async Task<IEnumerable<Script>> GetAll(
        ProjectId projectId,
        CancellationToken cancellationToken
    )
    {
        var scripts = await mongoRepository.GetAllDocuments(
            s => s.ProjectId == projectId.ToString() && s.Type == MongodbScriptType.File,
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

    public async Task<Script> Update(Script script, CancellationToken cancellationToken)
    {
        await mongoRepository.UpdateDocument(MongodbScript.From(script), cancellationToken);

        return script;
    }

    private async Task CreateParentFolder(
        MongodbScript mongodbScript,
        CancellationToken cancellationToken
    )
    {
        for (var i = 0; i < MaxDepth; i++)
        {
            var scriptFile = new ProjectStructureFile(mongodbScript.FilePath);
            var parentFolder = scriptFile.ParentPath;
            if (string.IsNullOrEmpty(parentFolder))
            {
                return;
            }

            var existingFolder = await mongoRepository.GetDocument(
                script => script.FilePath == parentFolder,
                cancellationToken
            );

            if (existingFolder is not null)
            {
                return;
            }

            var newFolder = new MongodbScript
            {
                Id = guidProvider.NewGuid().ToString(),
                ProjectId = mongodbScript.ProjectId,
                Type = MongodbScriptType.Folder,
                FilePath = parentFolder,
                ScriptLanguage = null,
                Parameters = null,
                ChildrenIds = [mongodbScript.Id],
            };
            await mongoRepository.CreateDocument(newFolder, cancellationToken);

            mongodbScript = newFolder;
        }
    }
}

using System.Linq.Expressions;
using MongoDB.Driver;
using OneOf;
using ScriptBee.Application.Model.Pagination;
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
) : ICreateScript, IGetScripts, IUpdateScript, IDeleteScript
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
        CancellationToken cancellationToken
    )
    {
        var result = await GetMongoFileEntry(scriptId, cancellationToken);

        return result.Match<OneOf<Script, ScriptDoesNotExistsError>>(
            script => script.ToScript(),
            error => error
        );
    }

    public async Task<Page<ProjectStructureEntry>> ListRootEntries(
        ProjectId projectId,
        int offset,
        int limit,
        CancellationToken cancellationToken
    )
    {
        Expression<Func<MongodbScript, bool>> expression = script =>
            script.ProjectId == projectId.Value && !script.FilePath.Contains('/');

        var totalCount = await mongoRepository.CountDocuments(expression, cancellationToken);
        var mongodbScripts = await mongoRepository.GetAllDocuments(
            expression,
            offset,
            limit,
            cancellationToken
        );

        return new Page<ProjectStructureEntry>(
            mongodbScripts.Select(s => s.ToProjectStructureEntry()),
            totalCount,
            offset,
            limit
        );
    }

    public async Task<OneOf<Page<ProjectStructureEntry>, ScriptDoesNotExistsError>> ListEntries(
        ProjectId projectId,
        ScriptId scriptId,
        int offset,
        int limit,
        CancellationToken cancellationToken
    )
    {
        var result = await GetMongoFileEntry(scriptId, cancellationToken);

        return await result.Match<
            Task<OneOf<Page<ProjectStructureEntry>, ScriptDoesNotExistsError>>
        >(
            async script => await ListEntries(script, offset, limit, cancellationToken),
            error =>
                Task.FromResult<OneOf<Page<ProjectStructureEntry>, ScriptDoesNotExistsError>>(error)
        );
    }

    public async Task<Script> Update(Script script, CancellationToken cancellationToken)
    {
        await mongoRepository.UpdateDocument(MongodbScript.From(script), cancellationToken);

        return script;
    }

    public async Task<ProjectStructureEntry?> Delete(
        ScriptId id,
        CancellationToken cancellationToken
    )
    {
        var mongodbScript = await mongoRepository.DeleteDocument(id.ToString(), cancellationToken);

        if (mongodbScript is null)
        {
            return null;
        }

        return await Delete(id, mongodbScript, cancellationToken);
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

    private async Task<ProjectStructureEntry?> Delete(
        ScriptId id,
        MongodbScript parentScript,
        CancellationToken cancellationToken
    )
    {
        var mongodbScript = await mongoRepository.DeleteDocument(id.ToString(), cancellationToken);

        if (mongodbScript is null)
        {
            return null;
        }

        var childrenIds = parentScript.ChildrenIds?.ToList() ?? [];

        foreach (var childrenId in childrenIds)
        {
            await Delete(new ScriptId(childrenId), parentScript, cancellationToken);
        }

        return parentScript.ToProjectStructureEntry();
    }

    private async Task<OneOf<MongodbScript, ScriptDoesNotExistsError>> GetMongoFileEntry(
        ScriptId scriptId,
        CancellationToken cancellationToken
    )
    {
        var mongodbScript = await mongoRepository.GetDocument(
            scriptId.ToString(),
            cancellationToken
        );

        return mongodbScript == null ? new ScriptDoesNotExistsError(scriptId) : mongodbScript;
    }

    private async Task<Page<ProjectStructureEntry>> ListEntries(
        MongodbScript script,
        int offset,
        int limit,
        CancellationToken cancellationToken
    )
    {
        if (script.Type == MongodbScriptType.File)
        {
            return new Page<ProjectStructureEntry>([script.ToScript()], 1, offset, limit);
        }

        var childrenIds = (script.ChildrenIds ?? []).ToList();
        var filteredIds = childrenIds.Skip(offset).Take(limit);

        var filter = Builders<MongodbScript>.Filter.In(x => x.Id, filteredIds);

        var mongodbScripts = await mongoRepository.GetAllDocuments(filter, cancellationToken);

        return new Page<ProjectStructureEntry>(
            mongodbScripts.Select(s => s.ToProjectStructureEntry()),
            childrenIds.Count,
            offset,
            limit
        );
    }
}

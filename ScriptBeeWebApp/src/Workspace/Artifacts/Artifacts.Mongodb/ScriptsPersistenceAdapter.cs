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

    public async Task<ScriptId?> Create(Script script, CancellationToken cancellationToken)
    {
        var mongodbScript = MongodbScript.From(script);
        await mongoRepository.CreateDocument(mongodbScript, cancellationToken);

        return await CreateParentFolder(mongodbScript, cancellationToken);
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

        var totalCount = await mongoRepository.MongoCollection.CountDocumentsAsync(
            expression,
            null,
            cancellationToken
        );

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

        var childrenIds = mongodbScript.ChildrenIds ?? [];

        foreach (var childrenId in childrenIds)
        {
            await Delete(new ScriptId(childrenId), cancellationToken);
        }

        return mongodbScript.ToProjectStructureEntry();
    }

    private async Task<ScriptId?> CreateParentFolder(
        MongodbScript mongodbScript,
        CancellationToken cancellationToken
    )
    {
        var scriptFile = new ProjectStructureFile(mongodbScript.FilePath);
        var parentPath = scriptFile.ParentPath;
        if (string.IsNullOrEmpty(parentPath))
        {
            return null;
        }

        var childId = mongodbScript.Id;
        var immediateParentPath = parentPath;
        string? createdImmediateParentId = null;

        for (var i = 0; i < MaxDepth; i++)
        {
            var existingFolder = await mongoRepository.GetDocument(
                script => script.FilePath == parentPath,
                cancellationToken
            );

            if (existingFolder is not null)
            {
                var children = (existingFolder.ChildrenIds ?? []).ToList();
                if (!children.Contains(childId))
                {
                    existingFolder.ChildrenIds = [.. children, childId];
                    await mongoRepository.UpdateDocument(existingFolder, cancellationToken);
                }

                if (parentPath == immediateParentPath)
                {
                    return new ScriptId(existingFolder.Id);
                }

                childId = existingFolder.Id;
                parentPath = new ProjectStructureFile(parentPath).ParentPath;
                if (string.IsNullOrEmpty(parentPath))
                {
                    return createdImmediateParentId is null
                        ? new ScriptId(childId)
                        : new ScriptId(createdImmediateParentId);
                }

                continue;
            }

            var newFolderId = guidProvider.NewGuid().ToString();
            var newFolder = new MongodbScript
            {
                Id = newFolderId,
                ProjectId = mongodbScript.ProjectId,
                Type = MongodbScriptType.Folder,
                FilePath = parentPath,
                ScriptLanguage = null,
                Parameters = null,
                ChildrenIds = [childId],
            };
            await mongoRepository.CreateDocument(newFolder, cancellationToken);

            if (parentPath == immediateParentPath)
            {
                createdImmediateParentId = newFolderId;
            }

            childId = newFolderId;
            parentPath = new ProjectStructureFile(parentPath).ParentPath;
            if (string.IsNullOrEmpty(parentPath))
            {
                return createdImmediateParentId is null
                    ? new ScriptId(newFolderId)
                    : new ScriptId(createdImmediateParentId);
            }
        }

        return createdImmediateParentId is null ? null : new ScriptId(createdImmediateParentId);
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

        var mongodbScripts = await mongoRepository
            .MongoCollection.Find(filter)
            .ToListAsync(cancellationToken);

        return new Page<ProjectStructureEntry>(
            mongodbScripts.Select(s => s.ToProjectStructureEntry()),
            childrenIds.Count,
            offset,
            limit
        );
    }
}

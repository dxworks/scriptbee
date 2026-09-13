using Microsoft.Extensions.Logging;
using OneOf;
using OneOf.Types;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.File;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Ports.Instance;
using ScriptBee.Ports.Project;
using ScriptBee.UseCases.Gateway.Context;

namespace ScriptBee.Service.Gateway.Context;

using LoadContextResult = OneOf<Success, ProjectDoesNotExistsError, InstanceDoesNotExistsError>;

public class LoadInstanceContextService(
    IGetProject getProject,
    IGetProjectInstance getProjectInstance,
    ILoadInstanceContext loadInstanceContext,
    IUpdateProject updateProject,
    ILogger<LoadInstanceContextService> logger
) : ILoadInstanceContextUseCase
{
    public async Task<LoadContextResult> Load(
        LoadContextCommand command,
        CancellationToken cancellationToken
    )
    {
        var result = await getProject.GetById(command.ProjectId, cancellationToken);

        return await result.Match<Task<LoadContextResult>>(
            details =>
                Load(
                    details,
                    command.InstanceId,
                    command.FilesToLoad,
                    command.LoaderIds,
                    cancellationToken
                ),
            error => Task.FromResult<LoadContextResult>(error)
        );
    }

    private async Task<LoadContextResult> Load(
        ProjectDetails projectDetails,
        InstanceId instanceId,
        IDictionary<string, List<string>>? filesToLoad,
        IEnumerable<string>? loaderIds,
        CancellationToken cancellationToken
    )
    {
        var result = await getProjectInstance.Get(instanceId, cancellationToken);

        return await result.Match<Task<LoadContextResult>>(
            async instanceInfo =>
            {
                await Load(projectDetails, instanceInfo, filesToLoad, loaderIds, cancellationToken);
                return new Success();
            },
            error => Task.FromResult<LoadContextResult>(error)
        );
    }

    private async Task Load(
        ProjectDetails projectDetails,
        InstanceInfo instanceInfo,
        IDictionary<string, List<string>>? filesToLoad,
        IEnumerable<string>? loaderIds,
        CancellationToken cancellationToken
    )
    {
        var resolvedFilesToLoad =
            filesToLoad != null
                ? GetFilesToLoadFromFileIds(projectDetails.SavedFiles, filesToLoad)
                : GetFilesToLoadFromLoaderIds(projectDetails.SavedFiles, loaderIds ?? []);

        var loaderSummary = string.Join(", ", resolvedFilesToLoad.Keys);
        logger.LogInformation(
            "Loading context for project {ProjectId} on instance {InstanceId} with loaders [{Loaders}]",
            projectDetails.Id,
            instanceInfo.Id,
            loaderSummary
        );

        await loadInstanceContext.Load(
            instanceInfo,
            GetLoadedFileIds(resolvedFilesToLoad),
            cancellationToken
        );
        await updateProject.Update(
            GetUpdateProjectDetailsWithLoadedFiles(projectDetails, resolvedFilesToLoad),
            cancellationToken
        );

        logger.LogInformation(
            "Context loaded for project {ProjectId} on instance {InstanceId}",
            projectDetails.Id,
            instanceInfo.Id
        );
    }

    private static Dictionary<string, List<FileData>> GetFilesToLoadFromFileIds(
        IEnumerable<FileData> savedFiles,
        IDictionary<string, List<string>> filesToLoad
    )
    {
        var savedFilesMap = savedFiles.ToDictionary(f => f.Id.ToString(), f => f);
        var result = new Dictionary<string, List<FileData>>();

        foreach (var (loaderId, fileIds) in filesToLoad)
        {
            var matchedFiles = new List<FileData>();
            foreach (var fileId in fileIds)
            {
                if (savedFilesMap.TryGetValue(fileId, out var fileData))
                {
                    matchedFiles.Add(fileData);
                }
            }

            result[loaderId] = matchedFiles;
        }

        return result;
    }

    private static Dictionary<string, List<FileData>> GetFilesToLoadFromLoaderIds(
        IEnumerable<FileData> savedFiles,
        IEnumerable<string> loaderIds
    )
    {
        var allFiles = savedFiles.ToList();
        return loaderIds.ToDictionary(loaderId => loaderId, _ => allFiles);
    }

    private static Dictionary<string, IEnumerable<FileId>> GetLoadedFileIds(
        Dictionary<string, List<FileData>> loadedFiles
    )
    {
        return loadedFiles.ToDictionary(x => x.Key, x => x.Value.Select(f => f.Id));
    }

    private static ProjectDetails GetUpdateProjectDetailsWithLoadedFiles(
        ProjectDetails projectDetails,
        Dictionary<string, List<FileData>> filesToLoad
    )
    {
        var updatedLoadedFiles = new Dictionary<string, List<FileData>>(projectDetails.LoadedFiles);

        foreach (var (loaderId, loadedFiles) in filesToLoad)
        {
            if (updatedLoadedFiles.TryGetValue(loaderId, out var existingLoadedFiles))
            {
                var combined = new List<FileData>(existingLoadedFiles);
                foreach (
                    var file in loadedFiles.Where(file => !combined.Any(f => f.Id.Equals(file.Id)))
                )
                {
                    combined.Add(file);
                }

                updatedLoadedFiles[loaderId] = combined;
            }
            else
            {
                updatedLoadedFiles[loaderId] = [.. loadedFiles];
            }
        }

        return projectDetails with
        {
            LoadedFiles = updatedLoadedFiles,
        };
    }
}

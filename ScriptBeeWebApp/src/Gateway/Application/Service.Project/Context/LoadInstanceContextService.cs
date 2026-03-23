using OneOf;
using ScriptBee.Domain.Model;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.File;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Ports.Instance;
using ScriptBee.Ports.Project;
using ScriptBee.UseCases.Project.Context;

namespace ScriptBee.Service.Project.Context;

using LoadContextResult = OneOf<Unit, ProjectDoesNotExistsError, InstanceDoesNotExistsError>;

public class LoadInstanceContextService(
    IGetProject getProject,
    IGetProjectInstance getProjectInstance,
    ILoadInstanceContext loadInstanceContext,
    IUpdateProject updateProject
) : ILoadInstanceContextUseCase
{
    public async Task<LoadContextResult> Load(
        LoadContextCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var result = await getProject.GetById(command.ProjectId, cancellationToken);

        return await result.Match<Task<LoadContextResult>>(
            details => Load(details, command.InstanceId, command.LoaderIds, cancellationToken),
            error => Task.FromResult<LoadContextResult>(error)
        );
    }

    private async Task<LoadContextResult> Load(
        ProjectDetails projectDetails,
        InstanceId instanceId,
        IEnumerable<string> loaderIds,
        CancellationToken cancellationToken
    )
    {
        var result = await getProjectInstance.Get(instanceId, cancellationToken);

        return await result.Match<Task<LoadContextResult>>(
            async instanceInfo =>
            {
                await Load(projectDetails, instanceInfo, loaderIds, cancellationToken);
                return new Unit();
            },
            error => Task.FromResult<LoadContextResult>(error)
        );
    }

    private async Task Load(
        ProjectDetails projectDetails,
        InstanceInfo instanceInfo,
        IEnumerable<string> loaderIds,
        CancellationToken cancellationToken
    )
    {
        var filesToLoad = GetFilesToLoad(projectDetails.SavedFiles, loaderIds.ToHashSet());
        await loadInstanceContext.Load(
            instanceInfo,
            GetLoadedFileIds(filesToLoad),
            cancellationToken
        );
        await updateProject.Update(
            GetUpdateProjectDetailsWithLoadedFiles(projectDetails, filesToLoad),
            cancellationToken
        );
    }

    private static Dictionary<string, List<FileData>> GetFilesToLoad(
        IDictionary<string, List<FileData>> savedFiles,
        HashSet<string> loaderIds
    )
    {
        var filesToLoad = new Dictionary<string, List<FileData>>();

        foreach (var keyValuePair in savedFiles)
        {
            if (loaderIds.Contains(keyValuePair.Key))
            {
                filesToLoad[keyValuePair.Key] = keyValuePair.Value;
            }
        }

        return filesToLoad;
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
            updatedLoadedFiles[loaderId] = loadedFiles;
        }

        return projectDetails with
        {
            LoadedFiles = updatedLoadedFiles,
        };
    }
}

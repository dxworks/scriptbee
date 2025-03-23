using OneOf;
using ScriptBee.Domain.Model;
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
    ILoadInstanceContext loadInstanceContext
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
        await loadInstanceContext.Load(
            instanceInfo,
            GetFilesToLoad(projectDetails.SavedFiles, loaderIds.ToHashSet()),
            cancellationToken
        );
    }

    private static Dictionary<string, IEnumerable<FileId>> GetFilesToLoad(
        IDictionary<string, List<FileData>> savedFiles,
        HashSet<string> loaderIds
    )
    {
        var filesToLoad = new Dictionary<string, IEnumerable<FileId>>();

        foreach (var keyValuePair in savedFiles)
        {
            if (loaderIds.Contains(keyValuePair.Key))
            {
                filesToLoad[keyValuePair.Key] = keyValuePair.Value.Select(data => data.Id);
            }
        }

        return filesToLoad;
    }
}

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

using UnloadContextResult = OneOf<Success, ProjectDoesNotExistsError, InstanceDoesNotExistsError>;

public class UnloadInstanceContextService(
    IGetProject getProject,
    IGetProjectInstance getProjectInstance,
    IClearInstanceContext clearInstanceContext,
    ILoadInstanceContext loadInstanceContext,
    ILinkInstanceContext linkInstanceContext,
    IUpdateProject updateProject
) : IUnloadInstanceContextUseCase
{
    public async Task<UnloadContextResult> Unload(
        UnloadInstanceContextCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var result = await getProject.GetById(command.ProjectId, cancellationToken);

        return await result.Match<Task<UnloadContextResult>>(
            async projectDetails =>
            {
                var instanceResult = await getProjectInstance.Get(
                    command.InstanceId,
                    cancellationToken
                );

                return await instanceResult.Match<Task<UnloadContextResult>>(
                    async instanceInfo =>
                    {
                        await UnloadFromProjectAndInstance(
                            projectDetails,
                            instanceInfo,
                            command.LoaderId,
                            command.FileId,
                            cancellationToken
                        );
                        return new Success();
                    },
                    error => Task.FromResult<UnloadContextResult>(error)
                );
            },
            error => Task.FromResult<UnloadContextResult>(error)
        );
    }

    private async Task UnloadFromProjectAndInstance(
        ProjectDetails projectDetails,
        InstanceInfo instanceInfo,
        string loaderId,
        FileId? fileId,
        CancellationToken cancellationToken
    )
    {
        var updatedLoadedFiles = new Dictionary<string, List<FileData>>(projectDetails.LoadedFiles);

        if (updatedLoadedFiles.TryGetValue(loaderId, out var loadedFiles))
        {
            if (fileId is not null)
            {
                loadedFiles.RemoveAll(f => f.Id.Equals(fileId));
                if (loadedFiles.Count == 0)
                {
                    updatedLoadedFiles.Remove(loaderId);
                }
            }
            else
            {
                updatedLoadedFiles.Remove(loaderId);
            }
        }

        var updatedProject = projectDetails with { LoadedFiles = updatedLoadedFiles };
        await updateProject.Update(updatedProject, cancellationToken);

        await clearInstanceContext.Clear(instanceInfo, cancellationToken);
        await loadInstanceContext.Load(
            instanceInfo,
            updatedProject.LoadedFiles.ToDictionary(x => x.Key, x => x.Value.Select(f => f.Id)),
            cancellationToken
        );
        await linkInstanceContext.Link(instanceInfo, updatedProject.Linkers, cancellationToken);
    }
}

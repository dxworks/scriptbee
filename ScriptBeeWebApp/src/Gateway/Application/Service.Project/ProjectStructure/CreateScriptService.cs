using OneOf;
using ScriptBee.Artifacts;
using ScriptBee.Common;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.ProjectStructure;
using ScriptBee.Ports.Plugins;
using ScriptBee.Ports.Project;
using ScriptBee.UseCases.Project.ProjectStructure;

namespace ScriptBee.Service.Project.ProjectStructure;

using CreateResult = OneOf<
    Script,
    ProjectDoesNotExistsError,
    NoInstanceAllocatedForProjectError,
    ScriptLanguageDoesNotExistsError,
    ScriptPathAlreadyExistsError
>;

public sealed class CreateScriptService(
    IGetProject getProject,
    IGetScriptLanguages getScriptLanguages,
    ICreateFile createFile,
    IGuidProvider guidProvider,
    ICreateScript createScript
) : ICreateScriptUseCase
{
    public async Task<CreateResult> Create(
        CreateScriptCommand command,
        CancellationToken cancellationToken
    )
    {
        var result = await getProject.GetById(command.ProjectId, cancellationToken);

        return await result.Match<Task<CreateResult>>(
            async details => await Create(command, details, cancellationToken),
            error => Task.FromResult<CreateResult>(error)
        );
    }

    private async Task<CreateResult> Create(
        CreateScriptCommand command,
        ProjectDetails projectDetails,
        CancellationToken cancellationToken
    )
    {
        // TODO FIXIT(#35): implement this without the current instance
        // var result = await currentInstanceUseCase.GetCurrentInstance(
        //     projectDetails.Id,
        //     cancellationToken
        // );

        return new NoInstanceAllocatedForProjectError(command.ProjectId);
        // return await result.Match<Task<CreateResult>>(
        //     async instanceInfo =>
        //         await Create(command, instanceInfo, projectDetails, cancellationToken),
        //     error => Task.FromResult<CreateResult>(error)
        // );
    }

    private async Task<CreateResult> Create(
        CreateScriptCommand command,
        InstanceInfo instanceInfo,
        ProjectDetails projectDetails,
        CancellationToken cancellationToken
    )
    {
        var languageResult = await getScriptLanguages.Get(
            instanceInfo,
            command.Language,
            cancellationToken
        );

        return await languageResult.Match<Task<CreateResult>>(
            async language => await Create(command, projectDetails, language, cancellationToken),
            error => Task.FromResult<CreateResult>(error)
        );
    }

    private async Task<CreateResult> Create(
        CreateScriptCommand command,
        ProjectDetails projectDetails,
        ScriptLanguage language,
        CancellationToken cancellationToken
    )
    {
        // TODO FIXIT(#35): add sample code to created file

        var createFileResult = await createFile.Create(
            projectDetails.Id,
            GetScriptPath(command.Path, language.Extension),
            "",
            cancellationToken
        );

        return await createFileResult.Match<Task<CreateResult>>(
            async result =>
                await Create(command, projectDetails, language, result, cancellationToken),
            error => Task.FromResult<CreateResult>(new ScriptPathAlreadyExistsError(error.Path))
        );
    }

    private static string GetScriptPath(string path, string languageExtension)
    {
        return path.EndsWith(languageExtension) ? path : path + languageExtension;
    }

    private async Task<CreateResult> Create(
        CreateScriptCommand command,
        ProjectDetails projectDetails,
        ScriptLanguage language,
        ProjectStructureFile file,
        CancellationToken cancellationToken
    )
    {
        var script = new Script(
            new ScriptId(guidProvider.NewGuid()),
            projectDetails.Id,
            file,
            language,
            command.Parameters
        );

        await createScript.Create(script, cancellationToken);

        return script;
    }
}

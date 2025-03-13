using OneOf;
using ScriptBee.Common;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.ProjectStructure;
using ScriptBee.Ports.Files;
using ScriptBee.Ports.Project;
using ScriptBee.Ports.Project.Structure;
using ScriptBee.UseCases.Project.ProjectStructure;

namespace ScriptBee.Service.Project.ProjectStructure;

using CreateResult = OneOf<
    Script,
    ProjectDoesNotExistsError,
    ScriptLanguageDoesNotExistsError,
    ScriptPathAlreadyExistsError
>;

public class CreateScriptService(
    IGetProject getProject,
    IGetScriptLanguages getScriptLanguages,
    ICreateFile createFile,
    IGuidProvider guidProvider,
    ICreateScript createScript
) : ICreateScriptUseCase
{
    public async Task<CreateResult> Create(
        CreateScriptCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var projectDetailsResult = await getProject.GetById(command.ProjectId, cancellationToken);

        return await projectDetailsResult.Match<Task<CreateResult>>(
            async details => await Create(command, details, cancellationToken),
            error => Task.FromResult<CreateResult>(error)
        );
    }

    private async Task<CreateResult> Create(
        CreateScriptCommand command,
        ProjectDetails projectDetails,
        CancellationToken cancellationToken = default
    )
    {
        var languageResult = await getScriptLanguages.Get(command.Language, cancellationToken);

        return await languageResult.Match<Task<CreateResult>>(
            async language => await Create(command, projectDetails, language, cancellationToken),
            error => Task.FromResult<CreateResult>(error)
        );
    }

    private async Task<CreateResult> Create(
        CreateScriptCommand command,
        ProjectDetails projectDetails,
        ScriptLanguage language,
        CancellationToken cancellationToken = default
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
        CreateFileResult createFileResult,
        CancellationToken cancellationToken = default
    )
    {
        var script = new Script(
            new ScriptId(guidProvider.NewGuid()),
            projectDetails.Id,
            createFileResult.Name,
            createFileResult.Path,
            createFileResult.AbsolutePath,
            language,
            command.Parameters
        );

        await createScript.Create(script, cancellationToken);

        return script;
    }
}

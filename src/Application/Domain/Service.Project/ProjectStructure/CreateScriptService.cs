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
        ProjectDetails details,
        CancellationToken cancellationToken = default
    )
    {
        var languageResult = await getScriptLanguages.Get(command.Language, cancellationToken);

        return await languageResult.Match<Task<CreateResult>>(
            async language => await Create(command, details, language, cancellationToken),
            error => Task.FromResult<CreateResult>(error)
        );
    }

    private async Task<CreateResult> Create(
        CreateScriptCommand command,
        ProjectDetails details,
        ScriptLanguage language,
        CancellationToken cancellationToken = default
    )
    {
        // TODO FIXIT(#35): add sample code to created file

        var createFileResult = await createFile.Create(
            GetScriptPath(command, details, language.Extension),
            cancellationToken
        );

        return await createFileResult.Match<Task<CreateResult>>(
            async result => await Create(command, details, language, result, cancellationToken),
            error => Task.FromResult<CreateResult>(new ScriptPathAlreadyExistsError(error.Path))
        );
    }

    private static string GetScriptPath(
        CreateScriptCommand command,
        ProjectDetails details,
        string languageExtension
    )
    {
        var path = command.Path.EndsWith(languageExtension)
            ? command.Path
            : command.Path + languageExtension;

        return Path.Combine(details.Id.ToString(), path);
    }

    private async Task<CreateResult> Create(
        CreateScriptCommand command,
        ProjectDetails details,
        ScriptLanguage language,
        CreateFileResult createFileResult,
        CancellationToken cancellationToken = default
    )
    {
        var script = new Script(
            new ScriptId(guidProvider.NewGuid()),
            details.Id,
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

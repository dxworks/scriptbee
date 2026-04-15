using DxWorks.ScriptBee.Plugin.Api;
using OneOf;
using ScriptBee.Artifacts;
using ScriptBee.Common;
using ScriptBee.Common.CodeGeneration;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.ProjectStructure;
using ScriptBee.Ports.Notifications;
using ScriptBee.Ports.Notifications.Events;
using ScriptBee.Ports.Project;
using ScriptBee.Service.Project.Plugin;
using ScriptBee.UseCases.Project.ProjectStructure;

namespace ScriptBee.Service.Project.ProjectStructure;

using CreateResult = OneOf<
    Script,
    ProjectDoesNotExistsError,
    ScriptLanguageDoesNotExistsError,
    ScriptPathAlreadyExistsError
>;

public sealed class CreateScriptService(
    IGetProject getProject,
    ICreateFile createFile,
    IGuidProvider guidProvider,
    ICreateScript createScript,
    IProjectNotificationsService projectNotificationsService,
    ScriptGeneratorStrategyFactory scriptGeneratorStrategyFactory
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
        var result = scriptGeneratorStrategyFactory.Get(command.Language);

        return await result.Match<Task<CreateResult>>(
            async language => await Create(command, projectDetails, language, cancellationToken),
            error =>
                Task.FromResult<CreateResult>(new ScriptLanguageDoesNotExistsError(error.Language))
        );
    }

    private async Task<CreateResult> Create(
        CreateScriptCommand command,
        ProjectDetails projectDetails,
        IScriptGeneratorStrategy strategy,
        CancellationToken cancellationToken
    )
    {
        var language = new ScriptLanguage(strategy.Language, strategy.Extension);

        var sampleCode = await new SampleCodeGenerator(
            strategy,
            new HashSet<string>()
        ).GenerateSampleCode(cancellationToken);

        var createFileResult = await createFile.Create(
            projectDetails.Id,
            GetScriptPath(command.Path, language.Extension),
            sampleCode,
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

        await projectNotificationsService.NotifyScriptCreated(
            new ScriptCreatedEvent(projectDetails.Id, script.Id),
            cancellationToken
        );

        return script;
    }
}

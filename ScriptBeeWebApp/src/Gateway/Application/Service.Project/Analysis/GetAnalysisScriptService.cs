using OneOf;
using ScriptBee.Analysis;
using ScriptBee.Artifacts;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.ProjectStructure;
using ScriptBee.UseCases.Project.Analysis;
using ScriptBee.UseCases.Project.ProjectStructure;

namespace ScriptBee.Service.Project.Analysis;

using GetScriptContentResult = OneOf<string, AnalysisDoesNotExistsError, ScriptDoesNotExistsError>;
using GetScriptMetadataResult = OneOf<Script, AnalysisDoesNotExistsError, ScriptDoesNotExistsError>;

public class GetAnalysisScriptService(
    IGetAnalysis getAnalysis,
    IFileModelService fileModelService,
    IGetScriptsUseCase getScriptsUseCase
) : IGetAnalysisScriptUseCase
{
    public async Task<GetScriptContentResult> GetScriptContent(
        AnalysisId analysisId,
        ScriptId scriptId,
        CancellationToken cancellationToken
    )
    {
        var result = await getAnalysis.GetById(analysisId, cancellationToken);
        return await result.Match<Task<GetScriptContentResult>>(
            async analysisInfo => await GetScriptContent(scriptId, analysisInfo, cancellationToken),
            error => Task.FromResult<GetScriptContentResult>(error)
        );
    }

    public async Task<GetScriptMetadataResult> GetFileScript(
        AnalysisId analysisId,
        ScriptId scriptId,
        CancellationToken cancellationToken
    )
    {
        var result = await getAnalysis.GetById(analysisId, cancellationToken);
        return await result.Match<Task<GetScriptMetadataResult>>(
            async analysisInfo => await GetScriptMetadata(analysisInfo, cancellationToken),
            error => Task.FromResult<GetScriptMetadataResult>(error)
        );
    }

    private async Task<GetScriptContentResult> GetScriptContent(
        ScriptId scriptId,
        AnalysisInfo analysisInfo,
        CancellationToken cancellationToken
    )
    {
        if (analysisInfo.ScriptFileId is null)
        {
            return new ScriptDoesNotExistsError(scriptId);
        }

        var stream = await fileModelService.GetFileAsync(
            analysisInfo.ScriptFileId.Value,
            cancellationToken
        );

        using var streamReader = new StreamReader(stream);
        return await streamReader.ReadToEndAsync(cancellationToken);
    }

    private async Task<GetScriptMetadataResult> GetScriptMetadata(
        AnalysisInfo analysisInfo,
        CancellationToken cancellationToken
    )
    {
        if (analysisInfo.ScriptFileId != null)
        {
            var metadata = await fileModelService.GetFileMetadataAsync<HistoricalScriptMetadata>(
                analysisInfo.ScriptFileId.Value,
                cancellationToken
            );

            if (metadata != null)
            {
                return new Script(
                    analysisInfo.ScriptId,
                    analysisInfo.ProjectId,
                    new ProjectStructureFile(metadata.Path),
                    new ScriptLanguage(metadata.LanguageName, metadata.LanguageExtension),
                    []
                );
            }
        }

        var result = await getScriptsUseCase.GetById(
            analysisInfo.ProjectId,
            analysisInfo.ScriptId,
            cancellationToken
        );

        return result.Match<GetScriptMetadataResult>(script => script, error => error);
    }
}

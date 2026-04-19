using OneOf;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.ProjectStructure;

namespace ScriptBee.UseCases.Gateway.Analysis;

public interface IGetAnalysisScriptUseCase
{
    Task<OneOf<string, AnalysisDoesNotExistsError, ScriptDoesNotExistsError>> GetScriptContent(
        AnalysisId analysisId,
        ScriptId scriptId,
        CancellationToken cancellationToken
    );

    Task<OneOf<Script, AnalysisDoesNotExistsError, ScriptDoesNotExistsError>> GetFileScript(
        AnalysisId analysisId,
        ScriptId scriptId,
        CancellationToken cancellationToken
    );
}

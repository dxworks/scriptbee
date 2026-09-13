using ScriptBee.Common.CodeGeneration;

namespace DxWorks.ScriptBee.Analysis.Sdk.Abstractions;

public interface IGenerateClassesUseCase
{
    Task<IEnumerable<SampleCodeFile>> GenerateClasses(
        List<string> languages,
        CancellationToken cancellationToken
    );
}

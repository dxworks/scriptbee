using ScriptBee.Common.CodeGeneration;

namespace ScriptBee.UseCases.Analysis;

public interface IGenerateClassesUseCase
{
    Task<IEnumerable<SampleCodeFile>> GenerateClasses(
        List<string> languages,
        CancellationToken cancellationToken
    );
}

namespace ScriptBee.UseCases.Analysis;

public interface IGenerateClassesUseCase
{
    Task GenerateClasses(CancellationToken cancellationToken);
}

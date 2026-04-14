using ScriptBee.Common.CodeGeneration;

namespace ScriptBee.Service.Analysis;

public interface IProjectStructureService
{
    public Task<IEnumerable<SampleCodeFile>> GenerateModelClasses(
        List<string> languages,
        CancellationToken cancellationToken
    );
}

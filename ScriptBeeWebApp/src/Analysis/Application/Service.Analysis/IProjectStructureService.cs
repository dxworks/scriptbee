namespace ScriptBee.Service.Analysis;

public interface IProjectStructureService
{
    public Task GenerateModelClasses(CancellationToken cancellationToken);
}

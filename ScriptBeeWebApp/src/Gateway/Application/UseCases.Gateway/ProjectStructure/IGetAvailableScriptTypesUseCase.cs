using ScriptBee.Domain.Model.ProjectStructure;

namespace ScriptBee.UseCases.Gateway.ProjectStructure;

public interface IGetAvailableScriptTypesUseCase
{
    IEnumerable<ScriptLanguage> GetAvailableScriptTypes();
}

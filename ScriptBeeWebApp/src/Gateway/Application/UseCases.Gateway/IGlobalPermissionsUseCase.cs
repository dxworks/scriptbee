namespace ScriptBee.UseCases.Gateway;

public interface IGlobalPermissionsUseCase
{
    Task<List<string>> GetGlobalPermissions(
        GetGlobalPermissionsQuery query,
        CancellationToken cancellationToken
    );
}

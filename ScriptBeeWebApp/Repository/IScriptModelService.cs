using ScriptBee.Models;

namespace ScriptBeeWebApp.Repository;

public interface IScriptModelService : IMongoService<ScriptModel>
{
    public Task<ScriptModel?> GetScriptModelByFilePathAsync(string filePath, string projectId,
        CancellationToken cancellationToken = default);
}

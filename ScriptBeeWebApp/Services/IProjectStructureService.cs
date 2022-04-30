using System.Threading;
using System.Threading.Tasks;
using ScriptBee.Models;

namespace ScriptBeeWebApp.Services;

public interface IProjectStructureService
{
    public Task<ProjectStructure> GetProjectStructure(string projectId, CancellationToken cancellationToken);

    public Task AddToProjectStructure(string projectId, string filePath, CancellationToken cancellationToken);
}
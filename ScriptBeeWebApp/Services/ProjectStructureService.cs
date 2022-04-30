using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using ScriptBee.Models;

namespace ScriptBeeWebApp.Services;

public class ProjectStructureService : IProjectStructureService
{
    private const string ProjectStructureCollectionName = "ProjectStructure";
    private readonly IMongoCollection<ProjectStructure> _mongoCollection;

    public ProjectStructureService(IMongoDatabase mongoDatabase)
    {
        _mongoCollection = mongoDatabase.GetCollection<ProjectStructure>(ProjectStructureCollectionName);
    }

    public async Task<ProjectStructure> GetProjectStructure(string projectId, CancellationToken cancellationToken)
    {
        var result = await _mongoCollection.Find(x => x.ProjectId == projectId).FirstOrDefaultAsync(cancellationToken);

        if (result == null)
        {
            await AddToProjectStructure(projectId, "src", cancellationToken);
            return CreateRootFolder(projectId);
        }

        return result;
    }

    public async Task AddToProjectStructure(string projectId, string filePath, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            return;
        }

        var projectStructure = await _mongoCollection
            .Find(x => x.ProjectId == projectId)
            .FirstOrDefaultAsync(cancellationToken);

        var pathParts = filePath.Split("/");

        if (projectStructure == null)
        {
            projectStructure = CreateRootFolder(projectId, pathParts[0]);
        }

        var currentNode = projectStructure.Nodes.FirstOrDefault(n => n.Name == pathParts[0]);
        if (currentNode == null)
        {
            currentNode = CreateRootFolder(projectId, pathParts[0]).Nodes[0];
        }

        var reconstructedPath = new StringBuilder(pathParts[0]);
        for (var index = 1; index < pathParts.Length; index++)
        {
            var pathPart = pathParts[index];
            reconstructedPath.Append('/');
            reconstructedPath.Append(pathPart);

            var childNode = currentNode.Children.FirstOrDefault(c => c.Name == pathPart);
            if (childNode == null)
            {
                childNode = new ProjectStructureNode(pathPart, new List<ProjectStructureNode>(),
                    reconstructedPath.ToString());
                currentNode.Children.Add(childNode);
            }

            currentNode = childNode;
        }

        await _mongoCollection.ReplaceOneAsync(x => x.ProjectId == projectId, projectStructure,
            new ReplaceOptions { IsUpsert = true }, cancellationToken);
    }

    private static ProjectStructure CreateRootFolder(string projectId, string folderName = "src")
    {
        return new ProjectStructure
        {
            ProjectId = projectId,
            Nodes = new List<ProjectStructureNode>
            {
                new(folderName, new List<ProjectStructureNode>(), folderName)
            }
        };
    }
}
using MongoDB.Driver;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.User;
using ScriptBee.Persistence.Mongodb.Entity;
using ScriptBee.Persistence.Mongodb.Repository;
using ScriptBee.Ports.Permissions;

namespace ScriptBee.Persistence.Mongodb;

public sealed class ProjectTokensPersistenceAdapter(
    IMongoRepository<MongodbProjectToken> mongoRepository
) : ICreateProjectToken, IGetAllProjectTokens, IDeleteProjectToken
{
    public async Task<ProjectToken> CreateToken(
        ProjectId projectId,
        string tokenHash,
        string? description,
        UserRole role,
        DateTimeOffset expiresAt,
        CancellationToken cancellationToken
    )
    {
        var mongoToken = new MongodbProjectToken
        {
            ProjectId = projectId.Value,
            Description = description,
            TokenHash = tokenHash,
            Role = role.Value,
            CreatedAt = DateTimeOffset.UtcNow,
            ExpiresAt = expiresAt,
        };

        await mongoRepository.CreateDocument(mongoToken, cancellationToken);

        return mongoToken.ToProjectToken();
    }

    public async Task<List<ProjectToken>> GetAllForProjectId(
        ProjectId projectId,
        CancellationToken cancellationToken
    )
    {
        var tokens = await mongoRepository.GetAllDocuments(
            token => token.ProjectId == projectId.Value,
            cancellationToken
        );
        return [.. tokens.Select(t => t.ToProjectToken())];
    }

    public async Task DeleteToken(
        ProjectId projectId,
        ProjectTokenId tokenId,
        CancellationToken cancellationToken
    )
    {
        var filter = Builders<MongodbProjectToken>.Filter.And(
            Builders<MongodbProjectToken>.Filter.Eq(m => m.ProjectId, projectId.Value),
            Builders<MongodbProjectToken>.Filter.Eq(m => m.Id, tokenId.Value)
        );

        await mongoRepository.MongoCollection.DeleteOneAsync(filter, cancellationToken);
    }
}

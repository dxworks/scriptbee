using MongoDB.Driver;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Persistence.Mongodb.Contracts;
using ScriptBee.Persistence.Mongodb.Repository;

namespace ScriptBee.Persistence.Mongodb.Tests;

public class ProjectInstancesPersistenceAdapterTest : IClassFixture<MongoDbFixture>
{
    private readonly ProjectInstancesPersistenceAdapter _adapter;
    private readonly IMongoCollection<ProjectInstance> _mongoCollection;

    public ProjectInstancesPersistenceAdapterTest(MongoDbFixture fixture)
    {
        _mongoCollection = fixture.GetCollection<ProjectInstance>("Instances");
        _adapter = new ProjectInstancesPersistenceAdapter(
            new MongoRepository<ProjectInstance>(_mongoCollection)
        );
    }

    [Fact]
    public async Task GetAllForProjectId()
    {
        var creationDate = DateTimeOffset.UtcNow;
        await _mongoCollection.InsertOneAsync(
            new ProjectInstance
            {
                Id = "all-instance-id-1",
                ProjectId = "all-project-id-1",
                Url = "http://test:80",
                CreationDate = creationDate,
            }
        );
        await _mongoCollection.InsertOneAsync(
            new ProjectInstance
            {
                Id = "all-instance-id-2",
                ProjectId = "all-project-id-2",
                Url = "http://test:80",
                CreationDate = creationDate,
            }
        );
        await _mongoCollection.InsertOneAsync(
            new ProjectInstance
            {
                Id = "all-instance-id-3",
                ProjectId = "all-project-id-1",
                Url = "http://test:80",
                CreationDate = creationDate,
            }
        );

        var instanceInfos = await _adapter.GetAll(
            ProjectId.FromValue("all-project-id-1"),
            CancellationToken.None
        );

        instanceInfos
            .ToList()
            .ShouldBeEquivalentTo(
                new List<InstanceInfo>
                {
                    new(
                        InstanceId.FromValue("all-instance-id-1"),
                        ProjectId.FromValue("all-project-id-1"),
                        "http://test:80",
                        creationDate
                    ),
                    new(
                        InstanceId.FromValue("all-instance-id-3"),
                        ProjectId.FromValue("all-project-id-1"),
                        "http://test:80",
                        creationDate
                    ),
                }
            );
    }
}

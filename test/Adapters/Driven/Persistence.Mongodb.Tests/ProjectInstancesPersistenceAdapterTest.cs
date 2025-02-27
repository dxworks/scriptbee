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
                Id = "834db275-e497-4f77-abd1-37c3bb3ba6de",
                ProjectId = "all-project-id-1",
                Url = "http://test:80",
                CreationDate = creationDate,
            }
        );
        await _mongoCollection.InsertOneAsync(
            new ProjectInstance
            {
                Id = "dce4de3e-ea47-4722-9742-9280ea18d38f",
                ProjectId = "all-project-id-2",
                Url = "http://test:80",
                CreationDate = creationDate,
            }
        );
        await _mongoCollection.InsertOneAsync(
            new ProjectInstance
            {
                Id = "47e981a7-9cd6-46d6-ba11-7f3c65ce38a2",
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
                        InstanceId.FromValue("834db275-e497-4f77-abd1-37c3bb3ba6de"),
                        ProjectId.FromValue("all-project-id-1"),
                        "http://test:80",
                        creationDate
                    ),
                    new(
                        InstanceId.FromValue("47e981a7-9cd6-46d6-ba11-7f3c65ce38a2"),
                        ProjectId.FromValue("all-project-id-1"),
                        "http://test:80",
                        creationDate
                    ),
                }
            );
    }
}

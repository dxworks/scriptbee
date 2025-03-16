using MongoDB.Driver;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Persistence.Mongodb.Entity;
using ScriptBee.Persistence.Mongodb.Repository;

namespace ScriptBee.Persistence.Mongodb.Tests;

public class ProjectInstancesPersistenceAdapterTest : IClassFixture<MongoDbFixture>
{
    private readonly ProjectInstancesPersistenceAdapter _adapter;
    private readonly IMongoCollection<MongodbProjectInstance> _mongoCollection;

    public ProjectInstancesPersistenceAdapterTest(MongoDbFixture fixture)
    {
        _mongoCollection = fixture.GetCollection<MongodbProjectInstance>("Instances");
        _adapter = new ProjectInstancesPersistenceAdapter(
            new MongoRepository<MongodbProjectInstance>(_mongoCollection)
        );
    }

    [Fact]
    public async Task GetAllForProjectId()
    {
        var creationDate = DateTimeOffset.UtcNow;
        await _mongoCollection.InsertOneAsync(
            new MongodbProjectInstance
            {
                Id = "834db275-e497-4f77-abd1-37c3bb3ba6de",
                ProjectId = "all-project-id-1",
                Url = "http://test:80",
                CreationDate = creationDate,
            }
        );
        await _mongoCollection.InsertOneAsync(
            new MongodbProjectInstance
            {
                Id = "dce4de3e-ea47-4722-9742-9280ea18d38f",
                ProjectId = "all-project-id-2",
                Url = "http://test:80",
                CreationDate = creationDate,
            }
        );
        await _mongoCollection.InsertOneAsync(
            new MongodbProjectInstance
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
                        new InstanceId("834db275-e497-4f77-abd1-37c3bb3ba6de"),
                        ProjectId.FromValue("all-project-id-1"),
                        "http://test:80",
                        creationDate
                    ),
                    new(
                        new InstanceId("47e981a7-9cd6-46d6-ba11-7f3c65ce38a2"),
                        ProjectId.FromValue("all-project-id-1"),
                        "http://test:80",
                        creationDate
                    ),
                }
            );
    }

    [Fact]
    public async Task GetById()
    {
        var creationDate = DateTimeOffset.UtcNow;
        await _mongoCollection.InsertOneAsync(
            new MongodbProjectInstance
            {
                Id = "e01cd721-d9ae-4bb1-8ff6-c8c3b89c6dc7",
                ProjectId = "project-id",
                Url = "http://test:80",
                CreationDate = creationDate,
            }
        );

        var instanceInfo = await _adapter.Get(
            new InstanceId("e01cd721-d9ae-4bb1-8ff6-c8c3b89c6dc7"),
            CancellationToken.None
        );

        instanceInfo.ShouldBe(
            new InstanceInfo(
                new InstanceId("e01cd721-d9ae-4bb1-8ff6-c8c3b89c6dc7"),
                ProjectId.FromValue("project-id"),
                "http://test:80",
                creationDate
            )
        );
    }

    [Fact]
    public async Task GivenNoInstanceForId_GetById_ShouldReturnInstanceDoesNotExistsError()
    {
        var instanceInfo = await _adapter.Get(
            new InstanceId("347c2d32-e639-4a87-ad56-1fb737485e41"),
            CancellationToken.None
        );

        instanceInfo.ShouldBe(
            new InstanceDoesNotExistsError(new InstanceId("347c2d32-e639-4a87-ad56-1fb737485e41"))
        );
    }
}

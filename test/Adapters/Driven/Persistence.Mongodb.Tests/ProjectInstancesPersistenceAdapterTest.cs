﻿using MongoDB.Driver;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Persistence.Mongodb.Entity;
using ScriptBee.Persistence.Mongodb.Repository;
using ScriptBee.Tests.Common;

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
    public async Task CreateNewInstance()
    {
        var instanceInfo = InstanceInfoFixture.BasicInstanceInfo(ProjectId.Create("id"));

        var result = await _adapter.Create(instanceInfo, CancellationToken.None);

        result.ShouldBe(instanceInfo);
        var createInstance = await _mongoCollection
            .Find(p => p.Id == instanceInfo.Id.ToString())
            .FirstOrDefaultAsync(cancellationToken: TestContext.Current.CancellationToken);
        createInstance.Id.ShouldBe(instanceInfo.Id.ToString());
        createInstance.ProjectId.ShouldBe("id");
        createInstance.Url.ShouldBe("http://instance");
        createInstance.CreationDate.ShouldBe(instanceInfo.CreationDate);
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
            },
            cancellationToken: TestContext.Current.CancellationToken
        );
        await _mongoCollection.InsertOneAsync(
            new MongodbProjectInstance
            {
                Id = "dce4de3e-ea47-4722-9742-9280ea18d38f",
                ProjectId = "all-project-id-2",
                Url = "http://test:80",
                CreationDate = creationDate,
            },
            cancellationToken: TestContext.Current.CancellationToken
        );
        await _mongoCollection.InsertOneAsync(
            new MongodbProjectInstance
            {
                Id = "47e981a7-9cd6-46d6-ba11-7f3c65ce38a2",
                ProjectId = "all-project-id-1",
                Url = "http://test:80",
                CreationDate = creationDate,
            },
            cancellationToken: TestContext.Current.CancellationToken
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
            },
            cancellationToken: TestContext.Current.CancellationToken
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

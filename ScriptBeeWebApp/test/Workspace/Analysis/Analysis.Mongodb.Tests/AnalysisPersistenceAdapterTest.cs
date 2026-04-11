using MongoDB.Driver;
using ScriptBee.Analysis.Mongodb.Entity;
using ScriptBee.Application.Model;
using ScriptBee.Application.Model.Sorting;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.File;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.ProjectStructure;
using ScriptBee.Persistence.Mongodb.Repository;
using ScriptBee.Tests.Common.Mongodb;

namespace ScriptBee.Analysis.Mongodb.Tests;

public class AnalysisPersistenceAdapterTest : IClassFixture<MongoDbFixture>
{
    private readonly AnalysisPersistenceAdapter _adapter;
    private readonly IMongoCollection<MongodbAnalysisInfo> _mongoCollection;

    public AnalysisPersistenceAdapterTest(MongoDbFixture fixture)
    {
        _mongoCollection = fixture.GetCollection<MongodbAnalysisInfo>("Analysis");
        _adapter = new AnalysisPersistenceAdapter(
            new MongoRepository<MongodbAnalysisInfo>(_mongoCollection)
        );
    }

    [Fact]
    public async Task GetAnalysisById()
    {
        var creationDate = DateTimeOffset.UtcNow;
        await _mongoCollection.InsertOneAsync(
            new MongodbAnalysisInfo
            {
                Id = "b2038fa2-75ef-4bb4-bb7a-9cc37725bf2c",
                ProjectId = "project-id-1",
                InstanceId = "85744120-93c6-4ad7-bbf9-6c815f73f576",
                ScriptId = "ffb278f2-4390-4f70-81ba-c51869f26385",
                ScriptFileId = "4d997302-09cf-43a2-8696-72e5d29cebe0",
                Status = AnalysisStatus.Started.Value,
                Errors = [],
                Results = [],
                CreationDate = creationDate,
                FinishedDate = null,
            },
            cancellationToken: TestContext.Current.CancellationToken
        );

        var result = await _adapter.GetById(
            new AnalysisId("b2038fa2-75ef-4bb4-bb7a-9cc37725bf2c"),
            TestContext.Current.CancellationToken
        );

        AssertAnalysisInfo(
            result.AsT0,
            new AnalysisInfo(
                new AnalysisId("b2038fa2-75ef-4bb4-bb7a-9cc37725bf2c"),
                ProjectId.FromValue("project-id-1"),
                new InstanceId("85744120-93c6-4ad7-bbf9-6c815f73f576"),
                new ScriptId("ffb278f2-4390-4f70-81ba-c51869f26385"),
                new FileId("4d997302-09cf-43a2-8696-72e5d29cebe0"),
                AnalysisStatus.Started,
                [],
                [],
                creationDate,
                null
            )
        );
    }

    [Fact]
    public async Task GivenNoAnalysis_GetAnalysisById_ShouldReturnAnalysisDoesNotExistsError()
    {
        var result = await _adapter.GetById(
            new AnalysisId("187c18d1-080c-4684-819f-9f9ffb30a99f"),
            TestContext.Current.CancellationToken
        );

        result.AsT1.ShouldBe(
            new AnalysisDoesNotExistsError(new AnalysisId("187c18d1-080c-4684-819f-9f9ffb30a99f"))
        );
    }

    [Fact]
    public async Task GetAllForProjectId_Descending()
    {
        var creationDate1 = DateTimeOffset.UtcNow.AddMinutes(-5);
        var creationDate2 = DateTimeOffset.UtcNow;
        await _mongoCollection.InsertOneAsync(
            new MongodbAnalysisInfo
            {
                Id = "834db275-e497-4f77-abd1-37c3bb3ba6de",
                ProjectId = "all-project-id-1",
                InstanceId = "4bc0b0cd-8192-413b-b7ff-364a7e3883ec",
                ScriptId = "e22be395-a668-4a26-81e7-67682afb1320",
                ScriptFileId = "f3444890-4d32-481c-83c9-1fb972b79040",
                Status = AnalysisStatus.Started.Value,
                Errors = [],
                Results = [],
                CreationDate = creationDate1,
                FinishedDate = null,
            },
            cancellationToken: TestContext.Current.CancellationToken
        );
        await _mongoCollection.InsertOneAsync(
            new MongodbAnalysisInfo
            {
                Id = "47e981a7-9cd6-46d6-ba11-7f3c65ce38a2",
                ProjectId = "all-project-id-1",
                InstanceId = "f03d2d88-bd3d-4b0a-90bf-3bc904523fd9",
                ScriptId = "f0c3287c-407c-4fda-9ac0-97e75be80c40",
                ScriptFileId = null,
                Status = AnalysisStatus.Finished.Value,
                Errors = [],
                Results = [],
                CreationDate = creationDate2,
                FinishedDate = creationDate2,
            },
            cancellationToken: TestContext.Current.CancellationToken
        );
        IReadOnlyList<AnalysisSort> sorts =
        [
            new(AnalysisSortField.CreationDate, SortOrder.Descending),
        ];

        var analysisInfos = await _adapter.GetAll(
            ProjectId.FromValue("all-project-id-1"),
            sorts,
            TestContext.Current.CancellationToken
        );

        var infos = analysisInfos.ToList();
        infos.Count.ShouldBe(2);
        infos[0].Id.Value.ShouldBe(Guid.Parse("47e981a7-9cd6-46d6-ba11-7f3c65ce38a2"));
        infos[1].Id.Value.ShouldBe(Guid.Parse("834db275-e497-4f77-abd1-37c3bb3ba6de"));
    }

    [Fact]
    public async Task GetAllForProjectId_Ascending()
    {
        var creationDate1 = DateTimeOffset.UtcNow.AddMinutes(-5);
        var creationDate2 = DateTimeOffset.UtcNow;
        await _mongoCollection.InsertOneAsync(
            new MongodbAnalysisInfo
            {
                Id = "476fa503-9a8e-4ef5-9941-9a51002d2af9",
                ProjectId = "all-project-id-asc",
                InstanceId = "4bc0b0cd-8192-413b-b7ff-364a7e3883ec",
                ScriptId = "e22be395-a668-4a26-81e7-67682afb1320",
                ScriptFileId = "f3444890-4d32-481c-83c9-1fb972b79040",
                Status = AnalysisStatus.Started.Value,
                Errors = [],
                Results = [],
                CreationDate = creationDate1,
                FinishedDate = null,
            },
            cancellationToken: TestContext.Current.CancellationToken
        );
        await _mongoCollection.InsertOneAsync(
            new MongodbAnalysisInfo
            {
                Id = "be867c86-0089-4814-8334-01ac1d92e504",
                ProjectId = "all-project-id-asc",
                InstanceId = "f03d2d88-bd3d-4b0a-90bf-3bc904523fd9",
                ScriptId = "f0c3287c-407c-4fda-9ac0-97e75be80c40",
                ScriptFileId = null,
                Status = AnalysisStatus.Finished.Value,
                Errors = [],
                Results = [],
                CreationDate = creationDate2,
                FinishedDate = creationDate2,
            },
            cancellationToken: TestContext.Current.CancellationToken
        );
        IReadOnlyList<AnalysisSort> sorts =
        [
            new(AnalysisSortField.CreationDate, SortOrder.Ascending),
        ];

        var analysisInfos = await _adapter.GetAll(
            ProjectId.FromValue("all-project-id-asc"),
            sorts,
            TestContext.Current.CancellationToken
        );

        var infos = analysisInfos.ToList();
        infos.Count.ShouldBe(2);
        infos[0].Id.Value.ShouldBe(Guid.Parse("476fa503-9a8e-4ef5-9941-9a51002d2af9"));
        infos[1].Id.Value.ShouldBe(Guid.Parse("be867c86-0089-4814-8334-01ac1d92e504"));
    }

    [Fact]
    public async Task CreateAnalysis()
    {
        var creationDate = DateTimeOffset.Now;
        var analysisInfo = AnalysisInfo.Started(
            new AnalysisId("15ed8fda-d6c8-49c9-909d-bf01d58b45b2"),
            ProjectId.FromValue("create-analysis-project"),
            new InstanceId("d267d2ab-b2b8-46bf-af24-ef070ee70f2e"),
            new ScriptId("f0078cb2-1ca2-48bf-be61-99b49b913d1a"),
            creationDate
        );

        var actualAnalysisInfo = await _adapter.Create(
            analysisInfo,
            TestContext.Current.CancellationToken
        );

        var expected = new AnalysisInfo(
            new AnalysisId("15ed8fda-d6c8-49c9-909d-bf01d58b45b2"),
            ProjectId.FromValue("create-analysis-project"),
            new InstanceId("d267d2ab-b2b8-46bf-af24-ef070ee70f2e"),
            new ScriptId("f0078cb2-1ca2-48bf-be61-99b49b913d1a"),
            null,
            AnalysisStatus.Started,
            [],
            [],
            creationDate,
            null
        );
        AssertAnalysisInfo(actualAnalysisInfo, expected);
        var foundAnalysisInfo = await _mongoCollection
            .Find(p => p.Id == "15ed8fda-d6c8-49c9-909d-bf01d58b45b2")
            .FirstOrDefaultAsync(cancellationToken: TestContext.Current.CancellationToken);
        AssertAnalysisInfo(foundAnalysisInfo.ToAnalysisInfo(), expected);
    }

    [Fact]
    public async Task DeleteAnalysis()
    {
        var creationDate = DateTimeOffset.UtcNow;
        await _mongoCollection.InsertOneAsync(
            new MongodbAnalysisInfo
            {
                Id = "5ab73da4-aebc-442a-afb2-639a1a7eca32",
                ProjectId = "delete-by-id",
                InstanceId = "f58f93a2-3626-4766-884e-d63e2bc3803f",
                ScriptId = "e88a2f54-5c71-4f2d-8678-ed58ead7a413",
                ScriptFileId = null,
                Status = AnalysisStatus.Finished.Value,
                Errors = [],
                Results = [],
                CreationDate = creationDate,
                FinishedDate = null,
            },
            cancellationToken: TestContext.Current.CancellationToken
        );

        await _adapter.DeleteById(
            new AnalysisId("5ab73da4-aebc-442a-afb2-639a1a7eca32"),
            TestContext.Current.CancellationToken
        );

        var analysisInfo = await _mongoCollection
            .Find(p => p.Id == "5ab73da4-aebc-442a-afb2-639a1a7eca32")
            .FirstOrDefaultAsync(cancellationToken: TestContext.Current.CancellationToken);
        analysisInfo.ShouldBeNull();
    }

    [Fact]
    public async Task GivenNotExistingId_DeleteById_ExpectNoError()
    {
        var analysisId = new AnalysisId("2c4ef6b0-8048-4440-bc65-55c3442a425b");

        await Should.NotThrowAsync(async () =>
            await _adapter.DeleteById(analysisId, TestContext.Current.CancellationToken)
        );
    }

    [Fact]
    public async Task DeleteAllAnalysisByProjectId()
    {
        var creationDate = DateTimeOffset.UtcNow;
        await _mongoCollection.InsertOneAsync(
            new MongodbAnalysisInfo
            {
                Id = "7c1c4a38-77fa-48b9-b37c-3dfa79177bb9",
                ProjectId = "delete-all-by-project-id-to-delete",
                InstanceId = "b57a27c6-afda-4d69-a016-2f2898c999f3",
                ScriptId = "0620b123-b467-4833-93b0-238a78d98cf1",
                ScriptFileId = null,
                Status = AnalysisStatus.Started.Value,
                Errors = [],
                Results = [],
                CreationDate = creationDate,
                FinishedDate = null,
            },
            cancellationToken: TestContext.Current.CancellationToken
        );
        await _mongoCollection.InsertOneAsync(
            new MongodbAnalysisInfo
            {
                Id = "bb5687c4-0290-48e0-8d17-ef9d8f7bc5bb",
                ProjectId = "delete-all-by-project-id",
                InstanceId = "b57a27c6-afda-4d69-a016-2f2898c999f3",
                ScriptId = "18a1b4f5-9d1e-4528-be97-0bce53981716",
                ScriptFileId = null,
                Status = AnalysisStatus.Finished.Value,
                Errors = [],
                Results = [],
                CreationDate = creationDate,
                FinishedDate = null,
            },
            cancellationToken: TestContext.Current.CancellationToken
        );

        await _adapter.DeleteAllByProjectId(
            ProjectId.FromValue("delete-all-by-project-id-to-delete"),
            TestContext.Current.CancellationToken
        );

        var deletedAnalysisInfo = await _mongoCollection
            .Find(p => p.Id == "7c1c4a38-77fa-48b9-b37c-3dfa79177bb9")
            .FirstOrDefaultAsync(cancellationToken: TestContext.Current.CancellationToken);
        deletedAnalysisInfo.ShouldBeNull();
        var remainingAnalysisInfo = await _mongoCollection
            .Find(p => p.Id == "bb5687c4-0290-48e0-8d17-ef9d8f7bc5bb")
            .FirstOrDefaultAsync(cancellationToken: TestContext.Current.CancellationToken);
        AssertAnalysisInfo(
            remainingAnalysisInfo.ToAnalysisInfo(),
            new AnalysisInfo(
                new AnalysisId("bb5687c4-0290-48e0-8d17-ef9d8f7bc5bb"),
                ProjectId.FromValue("delete-all-by-project-id"),
                new InstanceId("b57a27c6-afda-4d69-a016-2f2898c999f3"),
                new ScriptId("18a1b4f5-9d1e-4528-be97-0bce53981716"),
                null,
                AnalysisStatus.Finished,
                [],
                [],
                creationDate,
                null
            )
        );
    }

    [Fact]
    public async Task UpdateAnalysisInfo()
    {
        var creationDate = DateTimeOffset.UtcNow;
        await _mongoCollection.InsertOneAsync(
            new MongodbAnalysisInfo
            {
                Id = "ded96511-1a12-45ad-a866-97497593d122",
                ProjectId = "project-id-update",
                InstanceId = "856bbf5a-6767-493b-9f97-c5ba46dce837",
                ScriptId = "3e66bea9-648f-4154-acf8-d717b3ff1353",
                ScriptFileId = null,
                Status = AnalysisStatus.Started.Value,
                Errors = [],
                Results = [],
                CreationDate = creationDate,
                FinishedDate = null,
            },
            cancellationToken: TestContext.Current.CancellationToken
        );
        var updatedAnalysisInfo = new AnalysisInfo(
            new AnalysisId("ded96511-1a12-45ad-a866-97497593d122"),
            ProjectId.FromValue("project-id-update"),
            new InstanceId("856bbf5a-6767-493b-9f97-c5ba46dce837"),
            new ScriptId("3e66bea9-648f-4154-acf8-d717b3ff1353"),
            new FileId("b939f50d-c870-4a9b-a09f-6302d1a67b96"),
            AnalysisStatus.Finished,
            [],
            [],
            creationDate,
            null
        );

        var result = await _adapter.Update(
            updatedAnalysisInfo,
            TestContext.Current.CancellationToken
        );

        AssertAnalysisInfo(result, updatedAnalysisInfo);
        var foundAnalysisInfo = await _mongoCollection
            .Find(p => p.Id == "ded96511-1a12-45ad-a866-97497593d122")
            .FirstOrDefaultAsync(cancellationToken: TestContext.Current.CancellationToken);
        AssertAnalysisInfo(foundAnalysisInfo.ToAnalysisInfo(), updatedAnalysisInfo);
    }

    private static void AssertAnalysisInfo(AnalysisInfo actual, AnalysisInfo expected)
    {
        actual.Id.ShouldBe(expected.Id);
        actual.ProjectId.ShouldBe(expected.ProjectId);
        actual.Status.ShouldBe(expected.Status);
        actual.Errors.ToList().ShouldBe(expected.Errors.ToList());
        actual.Results.ToList().ShouldBe(expected.Results.ToList());
        actual.CreationDate.ShouldBe(expected.CreationDate);
        actual.FinishedDate.ShouldBe(expected.FinishedDate);
    }
}

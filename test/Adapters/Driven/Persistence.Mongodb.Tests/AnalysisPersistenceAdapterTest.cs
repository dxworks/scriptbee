using MongoDB.Driver;
using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.ProjectStructure;
using ScriptBee.Persistence.Mongodb.Entity.Analysis;
using ScriptBee.Persistence.Mongodb.Repository;

namespace ScriptBee.Persistence.Mongodb.Tests;

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
                ScriptId = "ffb278f2-4390-4f70-81ba-c51869f26385",
                Status = (int)AnalysisStatus.Started,
                Errors = [],
                Results = [],
                CreationDate = creationDate,
                FinishedDate = null,
            }
        );

        var result = await _adapter.GetById(
            new AnalysisId("b2038fa2-75ef-4bb4-bb7a-9cc37725bf2c"),
            CancellationToken.None
        );

        AssertAnalysisInfo(
            result.AsT0,
            new AnalysisInfo(
                new AnalysisId("b2038fa2-75ef-4bb4-bb7a-9cc37725bf2c"),
                ProjectId.FromValue("project-id-1"),
                new ScriptId("ffb278f2-4390-4f70-81ba-c51869f26385"),
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
            CancellationToken.None
        );

        result.ShouldBe(
            new AnalysisDoesNotExistsError(new AnalysisId("187c18d1-080c-4684-819f-9f9ffb30a99f"))
        );
    }

    [Fact]
    public async Task GetAllForProjectId()
    {
        var creationDate = DateTimeOffset.UtcNow;
        await _mongoCollection.InsertOneAsync(
            new MongodbAnalysisInfo
            {
                Id = "834db275-e497-4f77-abd1-37c3bb3ba6de",
                ProjectId = "all-project-id-1",
                ScriptId = "e22be395-a668-4a26-81e7-67682afb1320",
                Status = (int)AnalysisStatus.Started,
                Errors = [],
                Results = [],
                CreationDate = creationDate,
                FinishedDate = null,
            }
        );
        await _mongoCollection.InsertOneAsync(
            new MongodbAnalysisInfo
            {
                Id = "dce4de3e-ea47-4722-9742-9280ea18d38f",
                ProjectId = "all-project-id-2",
                ScriptId = "6c83d4b9-90a3-4800-baf1-dc23e01d915c",
                Status = (int)AnalysisStatus.InProgress,
                Errors = [],
                Results = [],
                CreationDate = creationDate,
                FinishedDate = null,
            }
        );
        await _mongoCollection.InsertOneAsync(
            new MongodbAnalysisInfo
            {
                Id = "47e981a7-9cd6-46d6-ba11-7f3c65ce38a2",
                ProjectId = "all-project-id-1",
                ScriptId = "f0c3287c-407c-4fda-9ac0-97e75be80c40",
                Status = (int)AnalysisStatus.Finished,
                Errors = [new MongodbAnalysisError { Message = "message" }],
                Results =
                [
                    new MongodbResultSummary
                    {
                        Id = "e2c95567-d0f5-48b8-b1b6-ccbf064219ce",
                        Type = "file",
                        CreationDate = creationDate,
                    },
                ],
                CreationDate = creationDate,
                FinishedDate = creationDate,
            }
        );

        var analysisInfos = await _adapter.GetAll(
            ProjectId.FromValue("all-project-id-1"),
            CancellationToken.None
        );

        var infos = analysisInfos.ToList();
        infos.Count.ShouldBe(2);
        AssertAnalysisInfo(
            infos[0],
            new AnalysisInfo(
                new AnalysisId("834db275-e497-4f77-abd1-37c3bb3ba6de"),
                ProjectId.FromValue("all-project-id-1"),
                new ScriptId("e22be395-a668-4a26-81e7-67682afb1320"),
                AnalysisStatus.Started,
                [],
                [],
                creationDate,
                null
            )
        );
        AssertAnalysisInfo(
            infos[1],
            new AnalysisInfo(
                new AnalysisId("47e981a7-9cd6-46d6-ba11-7f3c65ce38a2"),
                ProjectId.FromValue("all-project-id-1"),
                new ScriptId("f0c3287c-407c-4fda-9ac0-97e75be80c40"),
                AnalysisStatus.Finished,
                [
                    new ResultSummary(
                        new ResultId("e2c95567-d0f5-48b8-b1b6-ccbf064219ce"),
                        "file",
                        creationDate
                    ),
                ],
                [new AnalysisError("message")],
                creationDate,
                creationDate
            )
        );
    }

    [Fact]
    public async Task CreateAnalysis()
    {
        var creationDate = DateTimeOffset.Now;
        var analysisInfo = AnalysisInfo.Started(
            new AnalysisId("15ed8fda-d6c8-49c9-909d-bf01d58b45b2"),
            ProjectId.FromValue("create-analysis-project"),
            new ScriptId("f0078cb2-1ca2-48bf-be61-99b49b913d1a"),
            creationDate
        );

        var actualAnalysisInfo = await _adapter.Create(analysisInfo, CancellationToken.None);

        var expected = new AnalysisInfo(
            new AnalysisId("15ed8fda-d6c8-49c9-909d-bf01d58b45b2"),
            ProjectId.FromValue("create-analysis-project"),
            new ScriptId("f0078cb2-1ca2-48bf-be61-99b49b913d1a"),
            AnalysisStatus.Started,
            [],
            [],
            creationDate,
            null
        );
        AssertAnalysisInfo(actualAnalysisInfo, expected);
        var foundAnalysisInfo = await _mongoCollection
            .Find(p => p.Id == "15ed8fda-d6c8-49c9-909d-bf01d58b45b2")
            .FirstOrDefaultAsync();
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
                ScriptId = "e88a2f54-5c71-4f2d-8678-ed58ead7a413",
                Status = (int)AnalysisStatus.Finished,
                Errors = [],
                Results = [],
                CreationDate = creationDate,
                FinishedDate = null,
            }
        );

        await _adapter.DeleteById(
            new AnalysisId("5ab73da4-aebc-442a-afb2-639a1a7eca32"),
            CancellationToken.None
        );

        var analysisInfo = await _mongoCollection
            .Find(p => p.Id == "5ab73da4-aebc-442a-afb2-639a1a7eca32")
            .FirstOrDefaultAsync();
        analysisInfo.ShouldBeNull();
    }

    [Fact]
    public async Task GivenNotExistingId_DeleteById_ExpectNoError()
    {
        var analysisId = new AnalysisId("2c4ef6b0-8048-4440-bc65-55c3442a425b");

        await Should.NotThrowAsync(
            async () => await _adapter.DeleteById(analysisId, CancellationToken.None)
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
                ScriptId = "0620b123-b467-4833-93b0-238a78d98cf1",
                Status = (int)AnalysisStatus.Started,
                Errors = [],
                Results = [],
                CreationDate = creationDate,
                FinishedDate = null,
            }
        );
        await _mongoCollection.InsertOneAsync(
            new MongodbAnalysisInfo
            {
                Id = "bb5687c4-0290-48e0-8d17-ef9d8f7bc5bb",
                ProjectId = "delete-all-by-project-id",
                ScriptId = "18a1b4f5-9d1e-4528-be97-0bce53981716",
                Status = (int)AnalysisStatus.Finished,
                Errors = [],
                Results = [],
                CreationDate = creationDate,
                FinishedDate = null,
            }
        );

        await _adapter.DeleteAllByProjectId(
            ProjectId.FromValue("delete-all-by-project-id-to-delete"),
            CancellationToken.None
        );

        var deletedAnalysisInfo = await _mongoCollection
            .Find(p => p.Id == "7c1c4a38-77fa-48b9-b37c-3dfa79177bb9")
            .FirstOrDefaultAsync();
        deletedAnalysisInfo.ShouldBeNull();
        var remainingAnalysisInfo = await _mongoCollection
            .Find(p => p.Id == "bb5687c4-0290-48e0-8d17-ef9d8f7bc5bb")
            .FirstOrDefaultAsync();
        AssertAnalysisInfo(
            remainingAnalysisInfo.ToAnalysisInfo(),
            new AnalysisInfo(
                new AnalysisId("bb5687c4-0290-48e0-8d17-ef9d8f7bc5bb"),
                ProjectId.FromValue("delete-all-by-project-id"),
                new ScriptId("18a1b4f5-9d1e-4528-be97-0bce53981716"),
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
                ScriptId = "3e66bea9-648f-4154-acf8-d717b3ff1353",
                Status = (int)AnalysisStatus.Started,
                Errors = [],
                Results = [],
                CreationDate = creationDate,
                FinishedDate = null,
            }
        );
        var updatedAnalysisInfo = new AnalysisInfo(
            new AnalysisId("ded96511-1a12-45ad-a866-97497593d122"),
            ProjectId.FromValue("project-id-update"),
            new ScriptId("3e66bea9-648f-4154-acf8-d717b3ff1353"),
            AnalysisStatus.Finished,
            [],
            [],
            creationDate,
            null
        );

        var result = await _adapter.Update(updatedAnalysisInfo, CancellationToken.None);

        AssertAnalysisInfo(result, updatedAnalysisInfo);
        var foundAnalysisInfo = await _mongoCollection
            .Find(p => p.Id == "ded96511-1a12-45ad-a866-97497593d122")
            .FirstOrDefaultAsync();
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

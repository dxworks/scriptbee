using ScriptBee.Domain.Model.Project;
using ScriptBee.Domain.Model.ProjectStructure;

namespace ScriptBee.Artifacts.Mongodb.Tests;

public partial class ScriptPersistenceAdapterIntegrationTests
{
    [Fact]
    public async Task GetAllScripts()
    {
        var scriptId = Guid.NewGuid().ToString();
        await _mongoCollection.InsertOneAsync(
            CreateMongodbScript(scriptId, []),
            cancellationToken: TestContext.Current.CancellationToken
        );

        var result = await _adapter.GetAll(
            ProjectId.FromValue("id"),
            TestContext.Current.CancellationToken
        );

        result
            .Single(s => s.Id.Value.ToString() == scriptId)
            .AssertScript(CreateScript(new ScriptId(scriptId), []));
    }
}

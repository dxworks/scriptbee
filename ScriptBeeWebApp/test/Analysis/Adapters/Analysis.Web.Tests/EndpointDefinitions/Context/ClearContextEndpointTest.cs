using System.Net;
using DxWorks.ScriptBee.Analysis.Sdk.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using ScriptBee.Tests.Common;

namespace ScriptBee.Analysis.Web.Tests.EndpointDefinitions.Context;

public class ClearContextEndpointTest(ITestOutputHelper outputHelper)
{
    private const string TestUrl = "/api/context/clear";
    private readonly TestApiCaller<Program> _api = new(TestUrl);

    [Fact]
    public async Task ShouldClearContext()
    {
        var useCase = Substitute.For<IClearContextUseCase>();

        var response = await _api.PostApi<object>(
            new AnalysisTestWebApplicationFactory(
                outputHelper,
                services =>
                {
                    services.AddSingleton(useCase);
                }
            )
        );

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        useCase.Received(1).Clear();
    }
}

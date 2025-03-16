using System.Net;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using ScriptBee.Tests.Common;
using ScriptBee.UseCases.Analysis;
using Xunit.Abstractions;

namespace ScriptBee.Analysis.Web.Tests.EndpointDefinitions.Context;

public class ClearContextEndpointTest(ITestOutputHelper outputHelper)
{
    private const string TestUrl = "/api/context/clear";
    private readonly TestApiCaller<Program> _api = new(TestUrl);

    [Fact]
    public async Task ShouldClearContext()
    {
        var useCase = Substitute.For<IClearContextUseCase>();
        useCase.Clear(Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

        var response = await _api.PostApi<object>(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services =>
                {
                    services.AddSingleton(useCase);
                }
            )
        );

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }
}

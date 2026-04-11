using System.Net;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using ScriptBee.Tests.Common;
using ScriptBee.UseCases.Analysis;

namespace ScriptBee.Analysis.Web.Tests.EndpointDefinitions.Context;

public class GenerateClassesEndpointTest(ITestOutputHelper outputHelper)
{
    private const string TestUrl = "/api/context/generate-classes";
    private readonly TestApiCaller<Program> _api = new(TestUrl);

    [Fact]
    public async Task ShouldGenerateClasses()
    {
        var useCase = Substitute.For<IGenerateClassesUseCase>();

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
        await useCase.Received(1).GenerateClasses(Arg.Any<CancellationToken>());
    }
}

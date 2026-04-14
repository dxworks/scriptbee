using System.Net;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using ScriptBee.Analysis.Web.EndpointDefinitions.Context.Contracts;
using ScriptBee.Common.CodeGeneration;
using ScriptBee.Tests.Common;
using ScriptBee.UseCases.Analysis;

namespace ScriptBee.Analysis.Web.Tests.EndpointDefinitions.Context;

public class GenerateClassesEndpointTest(ITestOutputHelper outputHelper)
{
    private const string TestUrl = "/api/context/generate-classes";
    private readonly TestApiCaller<Program> _api = new(TestUrl);

    [Fact]
    public async Task ShouldGenerateAllClassesWithoutRequestBody()
    {
        var useCase = Substitute.For<IGenerateClassesUseCase>();
        useCase
            .GenerateClasses(Arg.Any<List<string>>(), Arg.Any<CancellationToken>())
            .Returns(new List<SampleCodeFile>());

        var response = await _api.PostApi<WebGenerateClassesRequest>(
            new AnalysisTestWebApplicationFactory(
                outputHelper,
                services =>
                {
                    services.AddSingleton(useCase);
                }
            ),
            new WebGenerateClassesRequest(null)
        );

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.ToString().ShouldBe("application/octet-stream");
        await useCase
            .Received(1)
            .GenerateClasses(Arg.Is<List<string>>(l => l.Count == 0), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ShouldGenerateSelectedClassesWithRequestBody()
    {
        var useCase = Substitute.For<IGenerateClassesUseCase>();
        var languages = new List<string> { "csharp", "python" };
        useCase
            .GenerateClasses(Arg.Any<List<string>>(), Arg.Any<CancellationToken>())
            .Returns(new List<SampleCodeFile>());

        var response = await _api.PostApi<WebGenerateClassesRequest>(
            new AnalysisTestWebApplicationFactory(
                outputHelper,
                services =>
                {
                    services.AddSingleton(useCase);
                }
            ),
            new WebGenerateClassesRequest(languages)
        );

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.ToString().ShouldBe("application/octet-stream");
        await useCase
            .Received(1)
            .GenerateClasses(
                Arg.Is<List<string>>(l => l.SequenceEqual(languages)),
                Arg.Any<CancellationToken>()
            );
    }
}

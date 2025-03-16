using System.Net;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using ScriptBee.Analysis.Web.EndpointDefinitions.Context.Contracts;
using ScriptBee.Tests.Common;
using ScriptBee.UseCases.Analysis;
using Xunit.Abstractions;
using static ScriptBee.Tests.Common.ProblemValidationUtils;

namespace ScriptBee.Analysis.Web.Tests.EndpointDefinitions.Context;

public class LinkContextEndpointTest(ITestOutputHelper outputHelper)
{
    private const string TestUrl = "/api/context/link";
    private readonly TestApiCaller<Program> _api = new(TestUrl);

    [Fact]
    public async Task InvalidRequestBody_ShouldReturnBadRequest()
    {
        var response = await _api.PostApi(
            new TestWebApplicationFactory<Program>(outputHelper),
            new WebLinkContextCommand(null!)
        );

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        await AssertValidationProblem(
            response.Content,
            TestUrl,
            new { LinkerIds = new List<string> { "'Linker Ids' must not be empty." } }
        );
    }

    [Fact]
    public async Task EmptyBody_ShouldReturnBadRequest()
    {
        var response = await _api.PostApi<WebLinkContextCommand>(
            new TestWebApplicationFactory<Program>(outputHelper)
        );

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        await AssertEmptyRequestBodyProblem(response.Content, TestUrl);
    }

    [Fact]
    public async Task ContexLinkSuccessful_ShouldReturnNoContent()
    {
        List<string> linkerIds = ["linker-id"];
        var useCase = Substitute.For<ILinkContextUseCase>();
        var response = await _api.PostApi(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services =>
                {
                    services.AddSingleton(useCase);
                }
            ),
            new WebLinkContextCommand(linkerIds)
        );

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        await useCase
            .Received(1)
            .Link(
                Arg.Is<IEnumerable<string>>(actual => actual.SequenceEqual(linkerIds)),
                Arg.Any<CancellationToken>()
            );
    }
}

using System.Net;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using ScriptBee.Analysis.Web.EndpointDefinitions.Context.Contracts;
using ScriptBee.Domain.Model.File;
using ScriptBee.Tests.Common;
using ScriptBee.UseCases.Analysis;
using Xunit.Abstractions;
using static ScriptBee.Tests.Common.ProblemValidationUtils;

namespace ScriptBee.Analysis.Web.Tests.EndpointDefinitions.Context;

public class LoadContextEndpointTest(ITestOutputHelper outputHelper)
{
    private const string TestUrl = "/api/context/load";
    private readonly TestApiCaller<Program> _api = new(TestUrl);

    [Fact]
    public async Task InvalidRequestBody_ShouldReturnBadRequest()
    {
        var response = await _api.PostApi(
            new TestWebApplicationFactory<Program>(outputHelper),
            new WebLoadContextCommand(null!)
        );

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        await AssertValidationProblem(
            response.Content,
            TestUrl,
            new { FilesToLoad = new List<string> { "'Files To Load' must not be empty." } }
        );
    }

    [Fact]
    public async Task EmptyBody_ShouldReturnBadRequest()
    {
        var response = await _api.PostApi<WebLoadContextCommand>(
            new TestWebApplicationFactory<Program>(outputHelper)
        );

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        await AssertEmptyRequestBodyProblem(response.Content, TestUrl);
    }

    [Fact]
    public async Task ContexLoadSuccessful_ShouldReturnNoContent()
    {
        var useCase = Substitute.For<ILoadContextUseCase>();
        var response = await _api.PostApi(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services =>
                {
                    services.AddSingleton(useCase);
                }
            ),
            new WebLoadContextCommand(
                new Dictionary<string, List<string>>
                {
                    { "loader-id", ["2d921470-7dca-4c00-877b-2bca8a1ed3da"] },
                }
            )
        );

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        await useCase
            .Received(1)
            .Load(
                Arg.Is<IDictionary<string, IEnumerable<FileId>>>(actual =>
                    actual.Count == 1
                    && actual["loader-id"]
                        .Single()
                        .Equals(new FileId("2d921470-7dca-4c00-877b-2bca8a1ed3da"))
                ),
                Arg.Any<CancellationToken>()
            );
    }
}

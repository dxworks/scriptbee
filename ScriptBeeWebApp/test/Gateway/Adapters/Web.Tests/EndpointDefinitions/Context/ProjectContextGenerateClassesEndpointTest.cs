using System.Net;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using OneOf;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Tests.Common;
using ScriptBee.UseCases.Project.Context;
using ScriptBee.Web.EndpointDefinitions.Context.Contracts;
using static ScriptBee.Tests.Common.ProblemValidationUtils;

namespace ScriptBee.Web.Tests.EndpointDefinitions.Context;

public class ProjectContextGenerateClassesEndpointTest(ITestOutputHelper outputHelper)
{
    private const string TestUrl =
        "/api/projects/project-id/instances/edd5a455-cc96-4701-9d7c-118c052aa965/context/generate-classes";

    private readonly TestApiCaller<Program> _api = new(TestUrl);

    [Fact]
    public async Task GenerateAllClassesSuccessful_WithoutRequestBody_ShouldReturnFileStream()
    {
        var projectId = ProjectId.FromValue("project-id");
        var instanceId = new InstanceId("edd5a455-cc96-4701-9d7c-118c052aa965");
        var useCase = Substitute.For<IGenerateInstanceClassesUseCase>();
        using var stream = new MemoryStream();
        useCase
            .Generate(
                Arg.Is<GenerateClassesCommand>(c =>
                    c.ProjectId == projectId && c.InstanceId == instanceId && c.Languages.Count == 0
                ),
                Arg.Any<CancellationToken>()
            )
            .Returns(Task.FromResult<OneOf<Stream, InstanceDoesNotExistsError>>(stream));

        var response = await _api.PostApi(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services =>
                {
                    services.AddSingleton(useCase);
                }
            ),
            new WebProjectContextGenerateClassesRequest(null)
        );

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.ToString().ShouldBe("application/octet-stream");
    }

    [Fact]
    public async Task GenerateSelectedClassesSuccessful_WithRequestBody_ShouldReturnFileStream()
    {
        var projectId = ProjectId.FromValue("project-id");
        var instanceId = new InstanceId("edd5a455-cc96-4701-9d7c-118c052aa965");
        var languages = new List<string> { "csharp" };
        var useCase = Substitute.For<IGenerateInstanceClassesUseCase>();
        using var stream = new MemoryStream();
        useCase
            .Generate(
                Arg.Is<GenerateClassesCommand>(c =>
                    c.ProjectId == projectId
                    && c.InstanceId == instanceId
                    && c.Languages.SequenceEqual(languages)
                ),
                Arg.Any<CancellationToken>()
            )
            .Returns(Task.FromResult<OneOf<Stream, InstanceDoesNotExistsError>>(stream));

        var response = await _api.PostApi(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services =>
                {
                    services.AddSingleton(useCase);
                }
            ),
            new WebProjectContextGenerateClassesRequest(languages)
        );

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.ToString().ShouldBe("application/octet-stream");
    }

    [Fact]
    public async Task InstanceNotExists_ShouldReturnNotFound()
    {
        var projectId = ProjectId.FromValue("project-id");
        var instanceId = new InstanceId("edd5a455-cc96-4701-9d7c-118c052aa965");
        var useCase = Substitute.For<IGenerateInstanceClassesUseCase>();
        useCase
            .Generate(
                Arg.Is<GenerateClassesCommand>(command =>
                    command.ProjectId == projectId
                    && command.InstanceId == instanceId
                    && command.Languages.Count == 0
                ),
                Arg.Any<CancellationToken>()
            )
            .Returns(
                Task.FromResult<OneOf<Stream, InstanceDoesNotExistsError>>(
                    new InstanceDoesNotExistsError(instanceId)
                )
            );

        var response = await _api.PostApi(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services =>
                {
                    services.AddSingleton(useCase);
                }
            ),
            new WebProjectContextGenerateClassesRequest(null)
        );

        await AssertInstanceNotFoundProblem(
            response,
            TestUrl,
            "edd5a455-cc96-4701-9d7c-118c052aa965"
        );
    }
}

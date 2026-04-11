using System.Net;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using OneOf;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Instance;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Tests.Common;
using ScriptBee.UseCases.Project.Analysis;
using static ScriptBee.Tests.Common.ProblemValidationUtils;

namespace ScriptBee.Web.Tests.EndpointDefinitions.Instances;

public class GetProjectInstanceByIdEndpointTest(ITestOutputHelper outputHelper)
{
    private const string TestUrl =
        "/api/projects/project-id/instances/b643bc2e-acca-4c55-b7ae-679fc67e252c";

    private readonly TestApiCaller<Program> _api = new(TestUrl);

    [Fact]
    public async Task GivenInstanceDoesNotExistsError_ShouldReturnNotFound()
    {
        var useCase = Substitute.For<IGetProjectInstancesUseCase>();
        var projectId = ProjectId.FromValue("project-id");
        var instanceId = new InstanceId("b643bc2e-acca-4c55-b7ae-679fc67e252c");
        useCase
            .GetInstance(projectId, instanceId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<InstanceInfo, InstanceDoesNotExistsError>>(
                    new InstanceDoesNotExistsError(instanceId)
                )
            );

        var response = await _api.GetApi(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services =>
                {
                    services.AddSingleton(useCase);
                }
            )
        );

        await AssertInstanceNotFoundProblem(
            response,
            TestUrl,
            "b643bc2e-acca-4c55-b7ae-679fc67e252c"
        );
    }

    [Theory]
    [FilePath("TestData/GetInstanceById/response.json")]
    public async Task GivenInstanceInfo_ShouldReturnOk(string responsePath)
    {
        var useCase = Substitute.For<IGetProjectInstancesUseCase>();
        var projectId = ProjectId.FromValue("project-id");
        var instanceId = new InstanceId("b643bc2e-acca-4c55-b7ae-679fc67e252c");
        var instanceInfo = InstanceInfoFixture.BasicInstanceInfo(projectId) with
        {
            Id = instanceId,
            CreationDate = DateTimeOffset.Parse("2026-01-01T00:00:00Z"),
            Status = AnalysisInstanceStatus.Running,
        };
        useCase
            .GetInstance(projectId, instanceId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<InstanceInfo, InstanceDoesNotExistsError>>(instanceInfo)
            );

        var response = await _api.GetApi(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services =>
                {
                    services.AddSingleton(useCase);
                }
            )
        );

        await response.AssertResponse(HttpStatusCode.OK, responsePath);
    }
}

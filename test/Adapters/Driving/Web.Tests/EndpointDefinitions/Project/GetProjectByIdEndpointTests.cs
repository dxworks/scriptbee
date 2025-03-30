﻿using System.Net;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using OneOf;
using ScriptBee.Domain.Model.Errors;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Tests.Common;
using ScriptBee.UseCases.Project;
using ScriptBee.Web.EndpointDefinitions.Project.Contracts;
using Xunit.Abstractions;
using static ScriptBee.Tests.Common.ProblemValidationUtils;
using static ScriptBee.Tests.Common.ProjectDetailsFixture;

namespace ScriptBee.Web.Tests.EndpointDefinitions.Project;

public class GetProjectByIdEndpointTests(ITestOutputHelper outputHelper)
{
    private const string TestUrl = "/api/projects/id";
    private readonly TestApiCaller<Program> _api = new(TestUrl);

    [Fact]
    public async Task ShouldReturnProjectDetails()
    {
        var projectId = ProjectId.FromValue("id");
        var query = new GetProjectQuery(projectId);
        var useCase = Substitute.For<IGetProjectsUseCase>();
        var creationDate = DateTimeOffset.Parse("2024-02-08");
        useCase
            .GetProject(query, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<ProjectDetails, ProjectDoesNotExistsError>>(
                    BasicProjectDetails(ProjectId.Create("id"), "name", creationDate)
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

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var getResponse = await response.ReadContentAsync<WebGetProjectDetailsResponse>();
        getResponse.ShouldBe(new WebGetProjectDetailsResponse("id", "name", creationDate));
    }

    [Fact]
    public async Task ProjectNotExists_ShouldReturnNotFound()
    {
        var projectId = ProjectId.FromValue("id");
        var query = new GetProjectQuery(projectId);
        var useCase = Substitute.For<IGetProjectsUseCase>();
        useCase
            .GetProject(query, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<OneOf<ProjectDetails, ProjectDoesNotExistsError>>(
                    new ProjectDoesNotExistsError(projectId)
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

        await AssertProjectNotFoundProblem(response, TestUrl, "id");
    }
}

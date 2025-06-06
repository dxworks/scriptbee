﻿using System.Net;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using ScriptBee.Domain.Model.File;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Tests.Common;
using ScriptBee.UseCases.Project;
using ScriptBee.Web.EndpointDefinitions.Project.Contracts;

namespace ScriptBee.Web.Tests.EndpointDefinitions.Project;

public class GetAllProjectsEndpointTests(ITestOutputHelper outputHelper)
{
    private const string TestUrl = "/api/projects";
    private readonly TestApiCaller<Program> _api = new(TestUrl);

    [Fact]
    public async Task ShouldReturnProjectDetailsList()
    {
        var getProjectsUseCase = Substitute.For<IGetProjectsUseCase>();
        var creationDate = DateTimeOffset.Parse("2024-02-08");
        IEnumerable<ProjectDetails> projectDetailsList = new List<ProjectDetails>
        {
            new(
                ProjectId.Create("id"),
                "name",
                creationDate,
                new Dictionary<string, List<FileData>>(),
                new Dictionary<string, List<FileData>>(),
                []
            ),
        };
        getProjectsUseCase
            .GetAllProjects(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(projectDetailsList));

        var response = await _api.GetApi(
            new TestWebApplicationFactory<Program>(
                outputHelper,
                services =>
                {
                    services.AddSingleton(getProjectsUseCase);
                }
            )
        );

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var getProjectListResponse = await response.ReadContentAsync<WebGetProjectListResponse>();
        getProjectListResponse.Projects.ShouldBeEquivalentTo(
            new List<WebGetProjectDetailsResponse> { new("id", "name", creationDate) }
        );
    }
}

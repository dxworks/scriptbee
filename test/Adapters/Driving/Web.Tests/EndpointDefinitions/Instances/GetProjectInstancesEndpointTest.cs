// TODO FIXIT: fix this
// using System.Net;
// using NSubstitute;
// using ScriptBee.Domain.Model.Analysis;
// using ScriptBee.Domain.Model.Project;
// using ScriptBee.Tests.Common;
// using ScriptBee.Web.EndpointDefinitions.Instances.Contracts;
// using Xunit.Abstractions;
//
// namespace ScriptBee.Web.Tests.EndpointDefinitions.Instances;
//
// public class GetProjectInstancesEndpointTest(ITestOutputHelper outputHelper)
// {
//     private const string TestUrl = "/api/projects/project-id/instances";
//     private readonly TestApiCaller _api = new(TestUrl);
//
//     [Fact]
//     public async Task ShouldReturnProjectDetailsList()
//     {
//         var getProjectInstances = Substitute.For<IGetProjectInstancesUseCase>();
//         var creationDate = DateTimeOffset.Parse("2024-02-08");
//         var projectId = ProjectId.Create("project-id");
//         IEnumerable<InstanceInfo> projectDetailsList = new List<InstanceInfo>
//         {
//             new(
//                 InstanceId.FromValue("instance-id"),
//                 projectId,
//                 "http://url",
//                 creationDate
//             ),
//         };
//         getProjectInstances
//             .GetAllInstances(projectId, Arg.Any<CancellationToken>())
//             .Returns(Task.FromResult(projectDetailsList));
//
//         var response = await _api.GetApi(
//             new TestWebApplicationFactory<Program>(
//                 outputHelper,
//                 services =>
//                 {
//                     services.AddSingleton(getProjectInstances);
//                 }
//             )
//         );
//
//         response.StatusCode.ShouldBe(HttpStatusCode.OK);
//         var getProjectListResponse =
//             await response.ReadContentAsync<WebGetProjectInstancesListResponse>();
//         getProjectListResponse.Instances.ShouldBeEquivalentTo(
//             new List<WebGetProjectInstance> { new("instance-id", creationDate) }
//         );
//     }
// }

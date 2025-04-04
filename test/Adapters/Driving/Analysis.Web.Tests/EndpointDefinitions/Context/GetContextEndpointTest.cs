using System.Net;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using ScriptBee.Analysis.Web.EndpointDefinitions.Context.Contracts;
using ScriptBee.Domain.Model.Context;
using ScriptBee.Tests.Common;
using ScriptBee.UseCases.Analysis;
using Xunit.Abstractions;

namespace ScriptBee.Analysis.Web.Tests.EndpointDefinitions.Context;

public class GetContextEndpointTest(ITestOutputHelper outputHelper)
{
    private const string TestUrl = "/api/context";
    private readonly TestApiCaller<Program> _api = new(TestUrl);

    [Fact]
    public async Task ShouldReturnContextSlices()
    {
        var useCase = Substitute.For<IGetContextUseCase>();
        useCase.Get().Returns([new ContextSlice("model", ["plugin-id"])]);

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
        var contextResponse = await response.ReadContentAsync<IEnumerable<WebContextSlice>>();
        var slice = contextResponse.ToList().Single();
        slice.Model.ShouldBe("model");
        slice.PluginIds.ShouldBeEquivalentTo(new List<string> { "plugin-id" });
    }
}

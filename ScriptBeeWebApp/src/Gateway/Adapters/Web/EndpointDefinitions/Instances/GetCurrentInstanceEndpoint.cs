using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Common.Web;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Service.Project.Analysis;
using ScriptBee.UseCases.Project.Analysis;
using ScriptBee.Web.EndpointDefinitions.Instances.Contracts;
using ScriptBee.Web.Exceptions;

namespace ScriptBee.Web.EndpointDefinitions.Instances;

using GetCurrentInstanceType = Results<Ok<WebProjectInstance>, NotFound<ProblemDetails>>;

public class GetCurrentInstanceEndpoint : IEndpointDefinition
{
    public void DefineServices(IServiceCollection services)
    {
        services.AddSingleton<IGetCurrentInstanceUseCase, GetCurrentInstanceService>();
    }

    public void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/projects/{projectId}/instances/current", GetCurrentInstance);
    }

    private static async Task<GetCurrentInstanceType> GetCurrentInstance(
        HttpContext context,
        [FromRoute] string projectId,
        IGetCurrentInstanceUseCase useCase,
        CancellationToken cancellationToken
    )
    {
        var result = await useCase.GetCurrentInstance(
            ProjectId.FromValue(projectId),
            cancellationToken
        );

        return result.Match<GetCurrentInstanceType>(
            instanceInfo => TypedResults.Ok(WebProjectInstance.Map(instanceInfo)),
            error => error.ToNotFoundProblem(context)
        );
    }
}

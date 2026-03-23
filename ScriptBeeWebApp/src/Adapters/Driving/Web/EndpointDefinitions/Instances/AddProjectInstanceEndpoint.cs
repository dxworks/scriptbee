using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Common.Web;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Service.Project.Analysis;
using ScriptBee.UseCases.Project.Analysis;
using ScriptBee.Web.EndpointDefinitions.Instances.Contracts;
using ScriptBee.Web.Exceptions;

namespace ScriptBee.Web.EndpointDefinitions.Instances;

using AllocateInstanceType = Results<Created<WebProjectInstance>, NotFound<ProblemDetails>>;

public class AddProjectInstanceEndpoint : IEndpointDefinition
{
    public void DefineServices(IServiceCollection services)
    {
        services.AddSingleton<IAllocateProjectInstanceUseCase, AllocateProjectInstanceService>();
    }

    public void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/projects/{projectId}/instances", AllocateInstance);
    }

    private static async Task<AllocateInstanceType> AllocateInstance(
        HttpContext context,
        [FromRoute] string projectId,
        IAllocateProjectInstanceUseCase useCase,
        CancellationToken cancellationToken = default
    )
    {
        var id = ProjectId.FromValue(projectId);
        var result = await useCase.Allocate(id, cancellationToken);

        return result.Match<AllocateInstanceType>(
            instanceInfo =>
                TypedResults.Created(
                    $"/api/projects/{projectId}/instances/{instanceInfo.Id}",
                    WebProjectInstance.Map(instanceInfo)
                ),
            error => error.ToProblem(context)
        );
    }
}

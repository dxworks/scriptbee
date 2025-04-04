﻿using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ScriptBee.Common.Web;
using ScriptBee.Domain.Model.Project;
using ScriptBee.Service.Project;
using ScriptBee.UseCases.Project;

namespace ScriptBee.Web.EndpointDefinitions.Project;

public class DeleteProjectEndpoint : IEndpointDefinition
{
    public void DefineServices(IServiceCollection services)
    {
        services.AddSingleton<IDeleteProjectUseCase, DeleteProjectService>();
    }

    public void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/projects/{projectId}", DeleteProject);
    }

    private static async Task<NoContent> DeleteProject(
        [FromRoute] string projectId,
        IDeleteProjectUseCase useCase,
        CancellationToken cancellationToken = default
    )
    {
        await useCase.DeleteProject(
            new DeleteProjectCommand(ProjectId.FromValue(projectId)),
            cancellationToken
        );

        return TypedResults.NoContent();
    }
}

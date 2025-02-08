﻿using ScriptBee.Domain.Model.Project;

namespace ScriptBee.Gateway.Web.EndpointDefinitions.Project.Contracts;

public record WebGetProjectDetailsResponse(string Id, string Name)
{
    public static WebGetProjectDetailsResponse Map(ProjectDetails projectDetails)
    {
        return new WebGetProjectDetailsResponse(projectDetails.Id.Value, projectDetails.Name);
    }
}

﻿using ScriptBee.Domain.Model.Instance;

namespace ScriptBee.Web.EndpointDefinitions.Instances.Contracts;

public record WebGetProjectInstancesListResponse(IEnumerable<WebProjectInstance> Instances)
{
    public static WebGetProjectInstancesListResponse Map(IEnumerable<InstanceInfo> instanceInfos)
    {
        return new WebGetProjectInstancesListResponse(instanceInfos.Select(WebProjectInstance.Map));
    }
}

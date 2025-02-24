using ScriptBee.Domain.Model.Calculation;

namespace ScriptBee.Gateway.Web.EndpointDefinitions.Instances.Contracts;

public record WebGetProjectInstancesListResponse(IEnumerable<WebGetProjectInstance> Instances)
{
    public static WebGetProjectInstancesListResponse Map(IEnumerable<CalculationInstanceInfo> instanceInfos)
    {
        return new WebGetProjectInstancesListResponse(instanceInfos.Select(WebGetProjectInstance.Map));
    }
}

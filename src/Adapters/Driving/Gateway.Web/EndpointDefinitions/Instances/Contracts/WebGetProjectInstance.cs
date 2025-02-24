using ScriptBee.Domain.Model.Calculation;

namespace ScriptBee.Gateway.Web.EndpointDefinitions.Instances.Contracts;

public record WebGetProjectInstance(string Id)
{
    public static WebGetProjectInstance Map(CalculationInstanceInfo info)
    {
        return new WebGetProjectInstance(info.Id.Value);
    }
}

using ScriptBee.Domain.Model.Analysis;

namespace ScriptBee.Web.EndpointDefinitions.Instances.Contracts;

public record WebGetProjectInstance(string Id, DateTimeOffset CreationDate)
{
    public static WebGetProjectInstance Map(InstanceInfo info)
    {
        return new WebGetProjectInstance(info.Id.ToString(), info.CreationDate);
    }
}

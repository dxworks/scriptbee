using ScriptBee.Domain.Model.Instance;

namespace ScriptBee.Web.EndpointDefinitions.Instances.Contracts;

public record WebProjectInstance(string Id, DateTimeOffset CreationDate)
{
    public static WebProjectInstance Map(InstanceInfo info)
    {
        return new WebProjectInstance(info.Id.ToString(), info.CreationDate);
    }
}

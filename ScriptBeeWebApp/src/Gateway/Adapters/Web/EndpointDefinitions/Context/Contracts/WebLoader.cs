using ScriptBee.Domain.Model.Context;

namespace ScriptBee.Web.EndpointDefinitions.Context.Contracts;

public record WebLoader(string Id, string Name)
{
    public static WebLoader Map(Loader loader)
    {
        return new WebLoader(loader.Id, loader.Name);
    }
}

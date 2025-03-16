using ScriptBee.Domain.Model.Context;

namespace ScriptBee.Web.EndpointDefinitions.Context.Contracts;

public record WebLinker(string Id, string Name)
{
    public static WebLinker Map(Linker linker)
    {
        return new WebLinker(linker.Id, linker.Name);
    }
}

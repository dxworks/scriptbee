using DxWorks.ScriptBee.Plugin.Api.Model;

namespace ScriptBee.Domain.Model.Context;

public class Project : IProject
{
    public IContext Context { get; set; } = new Context();
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public DateTimeOffset CreationDate { get; set; }
}

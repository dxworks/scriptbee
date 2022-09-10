using System;
using DxWorks.ScriptBee.Plugin.Api.Model;

namespace ScriptBee.ProjectContext;

public class Project : IProject
{
    public IContext Context { get; set; } = new Context();
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public DateTime CreationDate { get; set; }
}

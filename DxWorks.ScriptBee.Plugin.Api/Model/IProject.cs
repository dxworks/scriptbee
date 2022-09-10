using System;

namespace DxWorks.ScriptBee.Plugin.Api.Model;

public interface IProject
{
    public IContext Context { get; set; }
    public string Id { get; set; }
    public string Name { get; set; }
    public DateTime CreationDate { get; set; }
}

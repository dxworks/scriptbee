using System;

namespace ScriptBee.ProjectContext;

public class Project
{
    public Context Context { get; set; } = new();
    public string Id { get; set; }
    public string Name { get; set; }
    public DateTime CreationDate { get; set; }
}
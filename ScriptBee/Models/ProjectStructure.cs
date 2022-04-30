using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace ScriptBee.Models;

public class ProjectStructure
{
    [BsonId] public string ProjectId { get; set; }

    public List<ProjectStructureNode> Nodes { get; set; }
}

public record ProjectStructureNode(string Name, List<ProjectStructureNode> Children, string FilePath);

/*
public record ProjectStructureNode(string Name, string);
public record ProjectStructureChildNode(string Name, bool IsFolder);


List<Folders>
Folder {
     name: string
         children: {name: string, isFolder: bool} []
            parent: string
}

*/
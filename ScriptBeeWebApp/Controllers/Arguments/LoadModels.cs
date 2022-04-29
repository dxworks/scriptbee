using System.Collections.Generic;

namespace ScriptBeeWebApp.Controllers.Arguments;

public record LoadModels(string ProjectId, List<Node> Nodes);

public record Node(string LoaderName, List<string> Models);
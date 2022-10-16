using System.Collections.Generic;

namespace ScriptBee.ProjectContext;

public record FileTreeNode(string name, List<FileTreeNode>? children, string filePath, string srcPath);

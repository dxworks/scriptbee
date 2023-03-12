using System.Collections.Generic;

namespace ScriptBee.ProjectContext;

public record FileTreeNode(string Name, string FilePath, string SrcPath, List<FileTreeNode>? Children);

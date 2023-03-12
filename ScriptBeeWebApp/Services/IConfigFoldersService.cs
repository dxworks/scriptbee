namespace ScriptBeeWebApp.Services;

public interface IConfigFoldersService
{
    string GetAbsoluteFilePath(string projectId, string filePath);

    string GetProjectAbsolutePath(string projectId);

    string GetSrcRelativePath(string projectId, string filePath);
}

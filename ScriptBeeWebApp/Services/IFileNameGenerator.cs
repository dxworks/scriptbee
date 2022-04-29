namespace ScriptBeeWebApp.Services;

public interface IFileNameGenerator
{
    public string GenerateModelName(string projectId, string fileName, string loaderName);

    public string GenerateScriptName(string projectId, string filePath);

    public string GenerateOutputFileName(string projectId, string runId, string outputType, string fileName);

    public (string projectId, string fileName, string loaderName) ExtractModelNameComponents(string modelName);

    public (string projectId, string filePath) ExtractScriptNameComponents(string scriptName);

    public (string projectId, string runId, string outputType, string fileName) ExtractOutputFileNameComponents(
        string outputFileName);
}
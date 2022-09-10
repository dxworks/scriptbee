using DxWorks.ScriptBee.Plugin.Api.HelperFunctions;

namespace ScriptBeeWebApp.Services;

public class FileNameGenerator : IFileNameGenerator
{
    public string GenerateModelName(string projectId, string loaderName, string fileName)
    {
        return projectId + "|" + loaderName + "|" + fileName;
    }

    public string GenerateScriptName(string projectId, string filePath)
    {
        return projectId + "|" + filePath;
    }

    public string GenerateOutputFileName(string projectId, string runId, string outputType, string fileName)
    {
        return projectId + "|" + runId + "|" + outputType + "|" + fileName;
    }

    public (string projectId, string loaderName, string fileName) ExtractModelNameComponents(string modelName)
    {
        string[] components = modelName.Split("|");
        return (components[0], components[1], components[2]);
    }

    public (string projectId, string filePath) ExtractScriptNameComponents(string scriptName)
    {
        string[] components = scriptName.Split("|");
        return (components[0], components[1]);
    }

    public (string projectId, string runId, string outputType, string fileName) ExtractOutputFileNameComponents(
        string outputFileName)
    {
        string[] components = outputFileName.Split("|");
        return (components[0], components[1], components[2], components[3]);
    }
}
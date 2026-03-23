using ScriptBee.Domain.Model.Analysis;

namespace ScriptBee.Web.EndpointDefinitions.Analysis.Contracts;

public record WebGetAnalysisResultFileList(IEnumerable<WebGetAnalysisResultFile> Files)
{
    public static WebGetAnalysisResultFileList Map(IEnumerable<AnalysisFileResult> files)
    {
        return new WebGetAnalysisResultFileList(files.Select(WebGetAnalysisResultFile.Map));
    }
}

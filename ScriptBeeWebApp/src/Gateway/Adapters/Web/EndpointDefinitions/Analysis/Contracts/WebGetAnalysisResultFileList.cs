using ScriptBee.Domain.Model.Analysis;

namespace ScriptBee.Web.EndpointDefinitions.Analysis.Contracts;

public sealed record WebGetAnalysisResultFileList(IEnumerable<WebGetAnalysisResultFile> Data)
{
    public static WebGetAnalysisResultFileList Map(IEnumerable<AnalysisFileResult> files)
    {
        return new WebGetAnalysisResultFileList(files.Select(WebGetAnalysisResultFile.Map));
    }
}

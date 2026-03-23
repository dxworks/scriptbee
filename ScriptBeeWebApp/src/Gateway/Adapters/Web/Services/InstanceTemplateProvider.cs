using ScriptBee.Domain.Model.Analysis;
using ScriptBee.Ports.Instance;

namespace ScriptBee.Web.Services;

public class InstanceTemplateProvider(AnalysisInstanceImage analysisInstanceImage)
    : IInstanceTemplateProvider
{
    public AnalysisInstanceImage GetTemplate() => analysisInstanceImage;
}

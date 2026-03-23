using ScriptBee.Domain.Model.Analysis;

namespace ScriptBee.Ports.Instance;

public interface IInstanceTemplateProvider
{
    AnalysisInstanceImage GetTemplate();
}

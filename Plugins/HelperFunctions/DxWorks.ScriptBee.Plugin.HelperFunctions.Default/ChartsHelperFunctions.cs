using System.Text.Json;
using DxWorks.ScriptBee.Plugin.Api;
using DxWorks.ScriptBee.Plugin.Api.Services;
using DxWorks.ScriptBee.Plugin.HelperFunctions.Default.Charts;

namespace DxWorks.ScriptBee.Plugin.HelperFunctions.Default;

public class ChartsHelperFunctions(IHelperFunctionsResultService helperFunctionsResultService)
    : IHelperFunctions
{
    public void ExportBarChart(
        string name,
        List<Dictionary<string, object>> series,
        Dictionary<string, object>? options = null
    )
    {
        ExportChart(name, "bar", series, options);
    }

    public void ExportEChartsChart(
        string name,
        List<Dictionary<string, object>> series,
        Dictionary<string, object>? options = null
    )
    {
        ExportChart(name, "echarts", series, options);
    }

    private void ExportChart(
        string name,
        string type,
        List<Dictionary<string, object>> series,
        Dictionary<string, object>? options = null
    )
    {
        var chartData = JsonSerializer.Serialize(
            new ChartData
            {
                series = series,
                options = options ?? new Dictionary<string, object>(),
                type = type,
            }
        );

        helperFunctionsResultService.UploadResult(
            name + ".visualization",
            RunResultDefaultTypes.FileType,
            chartData
        );
    }
}

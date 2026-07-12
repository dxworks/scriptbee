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

    public void ExportBubbleChart(
        string name,
        List<Dictionary<string, object>> series,
        Dictionary<string, object>? options = null
    )
    {
        ExportChart(name, "bubble", series, options);
    }

    public void ExportGanttChart(
        string name,
        List<Dictionary<string, object>> series,
        Dictionary<string, object>? options = null
    )
    {
        ExportChart(name, "gantt", series, options);
    }

    public void ExportGraphChart(
        string name,
        List<Dictionary<string, object>> series,
        Dictionary<string, object>? options = null
    )
    {
        ExportChart(name, "graph", series, options);
    }

    public void ExportHeatmap(
        string name,
        List<Dictionary<string, object>> series,
        Dictionary<string, object>? options = null
    )
    {
        ExportChart(name, "heatmap", series, options);
    }

    public void ExportScatterPlot(
        string name,
        List<Dictionary<string, object>> series,
        Dictionary<string, object>? options = null
    )
    {
        ExportChart(name, "scatter-plot", series, options);
    }

    public void ExportTreeMap(
        string name,
        List<Dictionary<string, object>> series,
        Dictionary<string, object>? options = null
    )
    {
        ExportChart(name, "tree-map", series, options);
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

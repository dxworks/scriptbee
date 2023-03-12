using PactNet.Infrastructure.Outputters;
using Xunit.Abstractions;

namespace ScriptBeeWebApp.Tests.Contract.XUnitHelpers;

public class XUnitOutput : IOutput
{
    private readonly ITestOutputHelper _outputHelper;

    public XUnitOutput(ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
    }

    public void WriteLine(string line)
    {
        _outputHelper.WriteLine(line);
    }
}

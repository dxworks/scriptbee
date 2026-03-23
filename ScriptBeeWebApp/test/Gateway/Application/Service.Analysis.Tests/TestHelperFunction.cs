using DxWorks.ScriptBee.Plugin.Api;

namespace ScriptBee.Analysis.Service.Tests;

public class TestHelperFunction : IHelperFunctions
{
    public void TestFunction() { }

    public virtual void OverloadedFunction(string parameter) { }
}

public class OverloadedTestHelperFunction : TestHelperFunction
{
    public override void OverloadedFunction(string parameter) { }
}

using DxWorks.ScriptBee.Plugin.Api;

namespace DxWorks.ScriptBee.Plugin.ScriptRunner.CSharp.Tests;

internal class HelperFunctionWithoutGenericMethodsThatReturnSomething : IHelperFunctions
{
    public string GetSomething()
    {
        return "something";
    }

    public Something GetSomething(string a, int b, char c)
    {
        return new Something(a, b, c);
    }
}

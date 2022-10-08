using DxWorks.ScriptBee.Plugin.Api;

namespace DxWorks.ScriptBee.Plugin.ScriptRunner.CSharp.Tests;

internal class HelperFunctionWithoutGenericMethodThatReturnsVoid : IHelperFunctions
{
    public void Method1(string a, string b)
    {
    }
}

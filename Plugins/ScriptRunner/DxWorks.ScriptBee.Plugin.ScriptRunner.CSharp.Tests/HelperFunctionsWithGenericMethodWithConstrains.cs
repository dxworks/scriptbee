using DxWorks.ScriptBee.Plugin.Api;

namespace DxWorks.ScriptBee.Plugin.ScriptRunner.CSharp.Tests;

internal class HelperFunctionsWithGenericMethodWithConstrains : IHelperFunctions
{
    public void Method<T1, T2, T3, T4>()
        where T1 : class, new()
        where T2 : struct
        where T3 : Something
    {
    }

    public void Method<T1, T2, T3>()
        where T1 : class?
        where T2 : notnull, T1
        where T3 : unmanaged
    {
    }

    public void Method<T, TR>()
        where T : Something?
        where TR : Something, ISomething
    {
    }
}

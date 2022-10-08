using DxWorks.ScriptBee.Plugin.Api;

namespace DxWorks.ScriptBee.Plugin.ScriptRunner.CSharp.Tests;

internal class HelperFunctionsWithGenericMethods : IHelperFunctions
{
    public T? Method<T>()
    {
        return default;
    }

    public Something Method<T1, T2, T3>(T1? arg1, T2 arg2, T3 arg3)
    {
        return new Something(arg1.ToString(), arg2.ToString().Length, arg3.ToString()[0]);
    }
}

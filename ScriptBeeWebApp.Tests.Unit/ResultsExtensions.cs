using System;
using Microsoft.AspNetCore.Http;

namespace ScriptBeeWebApp.Tests.Unit;

// workaround until upgrade of AspnetCore version that contains OkObjectHttpResult
public static class ResultsExtensions
{
    public static T GetValue<T>(this IResult result)
    {
        return GetProperty<T>(result, "Value") ?? throw new ArgumentNullException();
    }

    public static int GetStatusCode(this IResult result)
    {
        return GetProperty<int?>(result, "StatusCode") ?? throw new ArgumentNullException();
    }

    public static T? GetProperty<T>(this IResult result, string propertyName)
    {
        return (T?)result.GetType().GetProperty(propertyName)?.GetValue(result);
    }
}

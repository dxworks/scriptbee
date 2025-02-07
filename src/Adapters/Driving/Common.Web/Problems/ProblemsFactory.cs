using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ScriptBee.Common.Web.Problems;

public static class ProblemsFactory
{
    public static ProblemHttpResult Conflict(string title, string detail)
    {
        return TypedResults.Problem(
            type: "https://tools.ietf.org/html/rfc9110#section-15.5.10",
            statusCode: (int)HttpStatusCode.Conflict,
            title: title,
            detail: detail
        );
    }
}

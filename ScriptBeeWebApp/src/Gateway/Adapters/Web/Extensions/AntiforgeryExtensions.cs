using Microsoft.AspNetCore.Antiforgery;

namespace ScriptBee.Web.Extensions;

public static class AntiforgeryExtensions
{
    public static IServiceCollection AddAntiforgeryHeader(this IServiceCollection services)
    {
        return services.AddAntiforgery(options =>
        {
            options.HeaderName = "X-XSRF-TOKEN";
        });
    }

    public static IApplicationBuilder UseAntiforgeryHeader(this IApplicationBuilder app)
    {
        return app.Use(
            async (context, next) =>
            {
                var antiforgery = context.RequestServices.GetRequiredService<IAntiforgery>();
                var tokens = antiforgery.GetAndStoreTokens(context);
                context.Response.Cookies.Append(
                    "XSRF-TOKEN",
                    tokens.RequestToken!,
                    new CookieOptions { HttpOnly = false, Path = "/" }
                );

                await next();
            }
        );
    }
}

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using ScriptBee.Web.Auth;
using ScriptBee.Web.Config;

namespace ScriptBee.Web.Extensions;

public static class AuthenticationExtensions
{
    private const string AuthenticationConfigSectionName = "Authentication";

    public static IServiceCollection AddAuthenticationConfig(
        this IServiceCollection services,
        ConfigurationManager configurationManager
    )
    {
        services
            .AddOptions<AuthenticationConfig>()
            .BindConfiguration(AuthenticationConfigSectionName);

        var authConfig = configurationManager
            .GetSection(AuthenticationConfigSectionName)
            .Get<AuthenticationConfig>()!;

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = authConfig.Authority;
                options.Audience = authConfig.Audience;
                options.RequireHttpsMetadata = authConfig.RequireHttpsMetadata;

                if (authConfig.IsDevelopment)
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                    };
                }
                else
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidIssuer = authConfig.Authority,
                        ValidAudience = authConfig.Audience,
                    };
                }
            });

        services.AddHttpContextAccessor();
        services.AddHttpClient(
            "OpaClient",
            client => client.BaseAddress = new Uri(GetOpaUrl(authConfig))
        );

        if (authConfig.IsDevelopment)
        {
            services.AddSingleton<IAuthorizationHandler, AllowAllAuthorizationHandler>();
        }
        else
        {
            services.AddSingleton<IAuthorizationHandler, AllowAllAuthorizationHandler>();
            // TODO FIXIT(#332): Add OPA authorization handler
            // services.AddSingleton<IAuthorizationHandler, OpaActionAuthorizationHandler>();
        }

        services.AddAuthorization();

        return services;
    }

    private static string GetOpaUrl(AuthenticationConfig config)
    {
        if (config.IsDevelopment)
        {
            return "";
        }

        return string.IsNullOrEmpty(config.OpaUrl)
            ? throw new InvalidOperationException(
                "OpaUrl is not configured and is mandatory. Please set Authentication:OpaUrl in your configuration."
            )
            : config.OpaUrl;
    }
}

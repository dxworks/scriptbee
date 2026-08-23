using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using ScriptBee.Web.Auth;
using ScriptBee.Web.Config;

namespace ScriptBee.Web.Extensions;

public static class AuthenticationExtensions
{
    private const string AuthenticationConfigSectionName = "Authentication";

    extension(IServiceCollection services)
    {
        public IServiceCollection AddAuthenticationConfig(ConfigurationManager configurationManager)
        {
            services
                .AddOptions<AuthenticationConfig>()
                .BindConfiguration(AuthenticationConfigSectionName);

            var authConfig = configurationManager
                .GetSection(AuthenticationConfigSectionName)
                .Get<AuthenticationConfig>()!;

            return services
                .AddAuthentication(authConfig)
                .AddAuthorizationServices(authConfig)
                .AddAuthorization();
        }

        private IServiceCollection AddAuthentication(AuthenticationConfig authConfig)
        {
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
            return services;
        }

        private IServiceCollection AddAuthorizationServices(AuthenticationConfig config)
        {
            if (config.IsDevelopment)
            {
                services.AddSingleton<IAuthorizationHandler, AllowAllAuthorizationHandler>();
            }
            else
            {
                services.AddSingleton<
                    IAuthorizationHandler,
                    ExternalAuthorizationActionAuthorizationHandler
                >();
            }

            services.AddHttpContextAccessor();
            services.AddHttpClient(
                AuthorizeExternally.ClientName,
                client => client.BaseAddress = new Uri(GetExternalAuthorizationUrl(config))
            );
            services.AddHttpClient(
                GetDefaultCreatorRole.ClientName,
                client => client.BaseAddress = new Uri(GetDefaultCreatorRoleUrl(config))
            );

            return services
                .AddSingleton<
                    IExternalAuthorizationContextProvider,
                    ExternalAuthorizationContextProvider
                >()
                .AddSingleton<IAuthorizeExternally, AuthorizeExternally>()
                .AddSingleton<IGetDefaultCreatorRole, GetDefaultCreatorRole>();
        }
    }

    private static string GetExternalAuthorizationUrl(AuthenticationConfig config)
    {
        if (config.IsDevelopment)
        {
            return "";
        }

        return string.IsNullOrEmpty(config.ExternalAuthorizationUrl)
            ? throw new InvalidOperationException(
                $"{nameof(config.ExternalAuthorizationUrl)} is not configured and is mandatory. Please set {AuthenticationConfigSectionName}:{nameof(config.ExternalAuthorizationUrl)} in your configuration."
            )
            : config.ExternalAuthorizationUrl;
    }

    private static string GetDefaultCreatorRoleUrl(AuthenticationConfig config)
    {
        if (config.IsDevelopment)
        {
            return "";
        }

        return string.IsNullOrEmpty(config.DefaultCreatorRoleUrl)
            ? throw new InvalidOperationException(
                $"{nameof(config.DefaultCreatorRoleUrl)} is not configured and is mandatory. Please set {AuthenticationConfigSectionName}:{nameof(config.DefaultCreatorRoleUrl)} in your configuration."
            )
            : config.DefaultCreatorRoleUrl;
    }
}

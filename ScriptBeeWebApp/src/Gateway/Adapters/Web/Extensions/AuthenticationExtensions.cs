using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using ScriptBee.Adapters.Auth;
using ScriptBee.Adapters.Auth.Config;
using ScriptBee.Adapters.Auth.Dev;
using ScriptBee.Ports.Permissions;

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
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var path = context.HttpContext.Request.Path;
                            if (
                                path.StartsWithSegments("/api/projectLiveUpdates")
                                && context.Request.Query.TryGetValue("access_token", out var token)
                            )
                            {
                                context.Token = token;
                            }

                            return Task.CompletedTask;
                        },
                    };

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
            services.AddHttpClientsFromConfigUrl(config);

            if (config.IsDevelopment)
            {
                services
                    .AddSingleton<IGetDefaultCreatorRole, GetDevAuthCreatorRole>()
                    .AddSingleton<IGetProjectPermissions, GetDevProjectPermissions>()
                    .AddSingleton<IGetResourceRole, GetDevResourceRole>();
            }
            else
            {
                services
                    .AddSingleton<IGetDefaultCreatorRole, GetDefaultCreatorRole>()
                    .AddSingleton<IGetProjectPermissions, GetProjectPermissions>();
            }

            return services
                .AddSingleton<
                    IExternalAuthorizationContextProvider,
                    ExternalAuthorizationContextProvider
                >()
                .AddSingleton<IAuthorizeExternally, AuthorizeExternally>();
        }

        private void AddHttpClientsFromConfigUrl(AuthenticationConfig config)
        {
            services.AddHttpClient(
                AuthorizeExternally.ClientName,
                client =>
                    client.BaseAddress = new Uri(
                        GetUrlFromConfig(
                            config,
                            c => c.ExternalAuthorizationUrl,
                            nameof(config.ExternalAuthorizationUrl)
                        )
                    )
            );
            services.AddHttpClient(
                GetDefaultCreatorRole.ClientName,
                client =>
                    client.BaseAddress = new Uri(
                        GetUrlFromConfig(
                            config,
                            c => c.DefaultCreatorRoleUrl,
                            nameof(config.DefaultCreatorRoleUrl)
                        )
                    )
            );
            services.AddHttpClient(
                GetProjectPermissions.ClientName,
                client =>
                    client.BaseAddress = new Uri(
                        GetUrlFromConfig(
                            config,
                            c => c.PermissionsUrl,
                            nameof(config.PermissionsUrl)
                        )
                    )
            );
            services.AddHttpClient(
                "GetRoles",
                client =>
                    client.BaseAddress = new Uri(
                        GetUrlFromConfig(config, c => c.RolesUrl, nameof(config.RolesUrl))
                    )
            );
        }
    }

    private static string GetUrlFromConfig(
        AuthenticationConfig config,
        Func<AuthenticationConfig, string?> func,
        string nameOfProperty
    )
    {
        if (config.IsDevelopment)
        {
            return "";
        }

        var url = func(config);

        return string.IsNullOrEmpty(url)
            ? throw new InvalidOperationException(
                $"{nameof(nameOfProperty)} is not configured and is mandatory. Please set {AuthenticationConfigSectionName}:{nameOfProperty} in your configuration."
            )
            : url;
    }
}

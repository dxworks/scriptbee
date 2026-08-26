using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Refit;
using ScriptBee.MCP.Auth;
using ScriptBee.MCP.Config;
using ScriptBee.MCP.Gateway.Generated;
using ScriptBee.MCP.Prompts;
using ScriptBee.MCP.Resources;
using ScriptBee.MCP.Tools;
using Serilog;

var switchMappings = new Dictionary<string, string>
{
    { "--token", "Authentication:AccessToken" },
    { "--client-id", "Authentication:ClientId" },
    { "--client-secret", "Authentication:ClientSecret" },
    { "--authority", "Authentication:Authority" },
    { "--scope", "Authentication:Scope" },
    { "--token-endpoint", "Authentication:TokenEndpoint" },
    { "--gateway-url", "GatewayApiUrl" },
};

if (args.Contains("--stdio"))
{
    await RunAsStdioServer(args, switchMappings);
    return;
}

await RunAsHttpServer(args, switchMappings);
return;

static async Task RunAsStdioServer(string[] args, Dictionary<string, string> switchMappings)
{
    var host = Host.CreateApplicationBuilder(args);
    host.Configuration.AddCommandLine(args, switchMappings);
    host.Configuration.AddEnvironmentVariables("ScriptBee__");

    host.Logging.ClearProviders();
    host.Logging.AddConsole(options => options.LogToStandardErrorThreshold = LogLevel.Warning);

    host.Services.AddServices(host.Configuration);

    await host.Build().RunAsync();
}

static async Task RunAsHttpServer(string[] args, Dictionary<string, string> switchMappings)
{
    Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();

    var builder = WebApplication.CreateBuilder(args);
    builder.Configuration.AddCommandLine(args, switchMappings);
    builder.Configuration.AddEnvironmentVariables("ScriptBee__");

    builder.Host.UseSerilog(
        (ctx, _, config) => config.ReadFrom.Configuration(ctx.Configuration).WriteTo.Console()
    );

    builder.Services.AddServices(builder.Configuration);

    var authConfig = builder.Configuration.GetSection(AuthConfig.SectionName).Get<AuthConfig>();
    var app = builder.Build();

    if (!string.IsNullOrWhiteSpace(authConfig?.Authority))
    {
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapMcp("/mcp").RequireAuthorization();
    }
    else
    {
        app.MapMcp("/mcp");
    }

    await app.RunAsync();
}

internal static class ServicesExtensions
{
    public static void AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AuthConfig>(configuration.GetSection(AuthConfig.SectionName));

        var authConfig = configuration.GetSection(AuthConfig.SectionName).Get<AuthConfig>();
        if (!string.IsNullOrWhiteSpace(authConfig?.Authority))
        {
            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = authConfig.Authority;
                    options.RequireHttpsMetadata = authConfig.RequireHttpsMetadata;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = false,
                        ValidIssuer = authConfig.Authority,
                    };
                });
            services.AddAuthorization();
        }

        services.AddHttpClient<IOidcTokenService, OidcTokenService>();
        services.AddTransient<GatewayAuthHandler>();

        services
            .AddRefitClient<IGatewayApi>()
            .ConfigureHttpClient(c =>
                c.BaseAddress = new Uri(configuration["GatewayApiUrl"] ?? "http://localhost:5117")
            )
            .AddHttpMessageHandler<GatewayAuthHandler>();

        services
            .AddMcpServer()
            .WithHttpTransport()
            .WithToolsFromAssembly(typeof(ProjectTools).Assembly)
            .WithPromptsFromAssembly(typeof(ScriptBeePrompts).Assembly)
            .WithResourcesFromAssembly(typeof(ScriptResources).Assembly);
    }
}

using Refit;
using ScriptBee.MCP.Gateway.Generated;
using ScriptBee.MCP.Prompts;
using ScriptBee.MCP.Resources;
using ScriptBee.MCP.Tools;
using Serilog;

if (args.Contains("--stdio"))
{
    await RunAsStdioServer(args);
    return;
}

await RunAsHttpServer(args);
return;

static async Task RunAsStdioServer(string[] args)
{
    var host = Host.CreateApplicationBuilder(args);

    host.Logging.ClearProviders();
    host.Logging.AddConsole(options => options.LogToStandardErrorThreshold = LogLevel.Warning);

    host.Services.AddServices(host.Configuration);

    await host.Build().RunAsync();
}

static async Task RunAsHttpServer(string[] args)
{
    Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog(
        (ctx, _, config) => config.ReadFrom.Configuration(ctx.Configuration).WriteTo.Console()
    );

    builder.Services.AddServices(builder.Configuration);

    var app = builder.Build();

    app.MapMcp("/mcp");

    await app.RunAsync();
}

internal static class ServicesExtensions
{
    public static void AddServices(
        this IServiceCollection services,
        ConfigurationManager configuration
    )
    {
        services
            .AddRefitClient<IGatewayApi>()
            .ConfigureHttpClient(c =>
                c.BaseAddress = new Uri(configuration["GatewayApiUrl"] ?? "http://localhost:5117")
            );

        services
            .AddMcpServer()
            .WithHttpTransport()
            .WithToolsFromAssembly(typeof(ProjectTools).Assembly)
            .WithPromptsFromAssembly(typeof(ScriptBeePrompts).Assembly)
            .WithResourcesFromAssembly(typeof(ScriptResources).Assembly);
    }
}

using FluentValidation;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using ScriptBee.Adapters.Notifications.SignalR;
using ScriptBee.Adapters.Notifications.SignalR.Hubs;
using ScriptBee.Artifacts.Extensions;
using ScriptBee.Common.Web;
using ScriptBee.Common.Web.EndpointDefinition;
using ScriptBee.Common.Web.Extensions;
using ScriptBee.Common.Web.Services;
using ScriptBee.Plugins.Marketplace.Extensions;
using ScriptBee.Rest.Extensions;
using ScriptBee.UseCases.Gateway.Plugins;
using ScriptBee.Web.EndpointDefinitions;
using ScriptBee.Web.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

var mongoConnectionString = builder.Configuration.GetConnectionString("mongodb");

builder
    .Services.AddConfiguredHealthChecks()
    .AddHttpContextAccessor()
    .AddSerilog()
    .AddAntiforgeryHeader()
    .AddOpenApi(options =>
    {
        options.AddDocumentTransformer(
            (document, _, _) =>
            {
                document.Info.Title = "ScriptBee Gateway API";
                document.Info.Version = "v2";
                document.Info.Description =
                    "API for ScriptBee Gateway service, managing projects, plugins, and analysis.";
                return Task.CompletedTask;
            }
        );

        options.StripWebPrefix();
        options.AddDescriptionSupport();
    })
    .AddValidatorsFromAssemblyContaining<IEndpointDefinitionMarker>()
    .AddProblemDetailsDefaults()
    .AddMongoDb(mongoConnectionString)
    .AddCommonServices()
    .AddArtifactFileAdapters()
    .AddRestConfig()
    .AddAnalysisConfig(builder.Configuration)
    .AddScriptBeeMarketplaceClient()
    .AddBackgroundServices("ScriptBee:Instance")
    .AddPluginServices("ScriptBee:Plugins")
    .AddProjectLiveUpdates();

builder.Services.AddEndpointDefinitions(
    typeof(IEndpointDefinition),
    typeof(IEndpointDefinitionMarker)
);

builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize = null;
});

builder.Host.UseSerilog();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapSwaggerUi();
}

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseRouting();
app.UseMiddleware<ClientIdMiddleware>();

app.UseAntiforgery();
app.UseAntiforgeryHeader();

app.MapHealthChecksEndpoint();

app.UseSerilogRequestLogging();

app.UseExceptionEndpoint();

app.UseEndpoints(_ => { });

app.MapHub<ProjectLiveUpdatesHub>("/api/projectLiveUpdates");

app.UseEndpointDefinitions();

app.MapFallbackToFile("index.html");

var pluginManager = app.Services.GetRequiredService<IManagePluginsUseCase>();

pluginManager.LoadPlugins();

app.Run();

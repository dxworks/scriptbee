using FluentValidation;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using ScriptBee.Adapters.Notifications.SignalR;
using ScriptBee.Adapters.Notifications.SignalR.Hubs;
using ScriptBee.Artifacts.Extensions;
using ScriptBee.Common.Web;
using ScriptBee.Common.Web.EndpointDefinition;
using ScriptBee.Common.Web.Extensions;
using ScriptBee.Common.Web.Services;
using ScriptBee.Marketplace.Client.Extensions;
using ScriptBee.Rest.Extensions;
using ScriptBee.UseCases.Project.Plugins;
using ScriptBee.Web.EndpointDefinitions;
using ScriptBee.Web.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

var mongoConnectionString = builder.Configuration.GetConnectionString("mongodb");

builder
    .Services.AddConfiguredHealthChecks()
    .AddHttpContextAccessor()
    .AddSerilog()
    .AddOpenApi()
    .AddValidatorsFromAssemblyContaining<IEndpointDefinitionMarker>()
    .AddProblemDetailsDefaults()
    .AddMongoDb(mongoConnectionString)
    .AddCommonServices()
    .AddArtifactFileAdapters("UserFolder")
    .AddRestConfig()
    .AddAnalysisConfig(builder.Configuration)
    .AddInstallPluginsForAllocatedInstancesServices()
    .AddScriptBeeMarketplaceClient()
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

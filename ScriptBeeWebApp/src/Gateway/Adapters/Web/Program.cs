using FluentValidation;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using ScriptBee.Artifacts.Extensions;
using ScriptBee.Common.Web;
using ScriptBee.Common.Web.EndpointDefinition;
using ScriptBee.Common.Web.Extensions;
using ScriptBee.Marketplace.Client.Extensions;
using ScriptBee.Rest.Extensions;
using ScriptBee.Web.EndpointDefinitions;
using ScriptBee.Web.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

var mongoConnectionString = builder.Configuration.GetConnectionString("mongodb");
var userFolderConfigurationSection = builder.Configuration.GetSection("UserFolder");

builder
    .Services.AddConfiguredHealthChecks()
    .AddSerilog()
    .AddOpenApi()
    .AddValidatorsFromAssemblyContaining<IEndpointDefinitionMarker>()
    .AddProblemDetailsDefaults()
    .AddMongoDb(mongoConnectionString)
    .AddCommonServices()
    .AddArtifactFileAdapters(userFolderConfigurationSection)
    .AddRestConfig()
    .AddAnalysisConfig(builder.Configuration)
    .AddScriptBeeMarketplaceClient()
    .AddInstallPluginsForAllocatedInstancesServices()
    // .AddFileWatcherServices()
    .AddSignalR();

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

app.MapHealthChecksEndpoint();

app.UseSerilogRequestLogging();

app.UseExceptionEndpoint();

app.UseEndpoints(_ => { });

app.UseEndpointDefinitions();

app.MapFallbackToFile("index.html");

app.Run();

public partial class Program;

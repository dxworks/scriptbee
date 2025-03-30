using FluentValidation;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using ScriptBee.Analysis.Web.EndpointDefinitions;
using ScriptBee.Analysis.Web.Extensions;
using ScriptBee.Common.Web;
using ScriptBee.Common.Web.EndpointDefinition;
using ScriptBee.Common.Web.Extensions;
using ScriptBee.Persistence.File.Extensions;
using ScriptBee.UseCases.Plugin;
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
    .AddFileAdapters(userFolderConfigurationSection)
    .AddPluginsConfig()
    .AddProjectContextConfig()
    .AddRunScriptServices();

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

app.UseStaticFiles();
app.UseRouting();

app.MapHealthChecksEndpoint();

app.UseSerilogRequestLogging();

app.UseExceptionEndpoint();

app.UseEndpoints(_ => { });

app.UseEndpointDefinitions();

var pluginManager = app.Services.GetRequiredService<IManagePluginsUseCase>();

pluginManager.LoadPlugins();

app.Run();

public partial class Program;

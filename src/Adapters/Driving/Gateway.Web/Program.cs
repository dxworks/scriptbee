// TODO: uncomment and implement when functionality is refactored
// using ScriptBee.Marketplace.Client;
// using ScriptBee.Plugin;
// using ScriptBeeWebApp.Hubs;

using FluentValidation;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using ScriptBee.Common.Web;
using ScriptBee.Common.Web.Extensions;
using ScriptBee.Gateway.Web.Config;
using ScriptBee.Gateway.Web.EndpointDefinitions;
using ScriptBee.Gateway.Web.EndpointDefinitions.Validation;
using ScriptBee.Gateway.Web.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

var mongoConnectionString = builder.Configuration.GetConnectionString("mongodb");
// var userFolderConfigurationSection = builder.Configuration.GetSection("UserFolder");
var jwtSettings = builder.Configuration.GetRequiredSection("Jwt").Get<JwtSettings>()!;
var featuresSettings = builder.Configuration.GetSection("Features").Get<FeaturesSettings>() ?? new FeaturesSettings();

// TODO: move service registration in Endpoint Definitions 

builder.Services
    .AddAuthorizationRules()
    .AddJwtAuthentication(jwtSettings)
    .AddSerilog()
    .AddOpenApi()
    .AddValidatorsFromAssemblyContaining<IValidationMarker>()
    .AddProblemDetailsDefaults()
    .AddMongoDb(mongoConnectionString)
    .AddCommonServices()
    // .AddPluginServices()
    // .AddScriptBeeMarketplaceClient()
    // .AddFileWatcherServices()
    // .AddControllerServices(userFolderConfigurationSection)
    .AddSignalR();

builder.Services.AddEndpointDefinitions(typeof(IEndpointDefinition), typeof(IEndpointDefinitionMarker));

builder.Services.Configure<KestrelServerOptions>(options => { options.Limits.MaxRequestBodySize = null; });

builder.Host.UseSerilog();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapSwaggerUi();
}

if (!featuresSettings.DisableAuthorization)
{
    app.UseAuthentication();
}

app.UseStaticFiles();
app.UseRouting();

app.UseSerilogRequestLogging();

app.UseExceptionEndpoint();

if (!featuresSettings.DisableAuthorization)
{
    app.UseAuthorization();
}

app.UseEndpoints(_ => { });

// app.UseSpa(spa =>
// {
//     spa.Options.SourcePath = "../ScriptBeeClient";
//
//     if (app.Environment.IsDevelopment())
//     {
//         spa.UseProxyToSpaDevelopmentServer("http://localhost:4200");
//     }
// });

// app.MapHub<FileWatcherHub>("/api/fileWatcherHub");

app.UseEndpointDefinitions(builder.Configuration);

// var pluginManager = app.Services.GetRequiredService<PluginManager>();

// todo move to task
// pluginManager.LoadPlugins();

app.Run();

public partial class Program;

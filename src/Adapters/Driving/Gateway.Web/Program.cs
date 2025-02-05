// TODO: uncomment and implement when functionality is refactored
// using FluentValidation;
// using ScriptBee.Marketplace.Client;
// using ScriptBee.Plugin;
// using ScriptBeeWebApp.EndpointDefinitions.Arguments.Validation;
// using ScriptBeeWebApp.Extensions;
// using ScriptBeeWebApp.Hubs;

using Microsoft.AspNetCore.Server.Kestrel.Core;
using ScriptBee.Common.Web;
using ScriptBee.Common.Web.Extensions;
using ScriptBee.Gateway.Web.EndpointDefinitions;
using ScriptBee.Gateway.Web.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// var mongoConnectionString = builder.Configuration.GetConnectionString("mongodb");
// var userFolderConfigurationSection = builder.Configuration.GetSection("UserFolder");

// TODO: move service registration in Endpoint Definitions 
builder.Services
    .AddAuthorizationRules()
    .AddJwtAuthentication(builder.Configuration)
    .AddSerilog()
    .AddOpenApi()
    // .AddMongoDb(mongoConnectionString)
    // .AddUtilityServices()
    // .AddPluginServices()
    // .AddScriptBeeMarketplaceClient()
    // .AddFileWatcherServices()
    // .AddControllerServices(userFolderConfigurationSection)
    // .AddValidatorsFromAssemblyContaining<IValidationMarker>()
    .AddSignalR();

builder.Services.AddEndpointDefinitions(typeof(IEndpointDefinition), typeof(IEndpointDefinitionMarker));

builder.Services.Configure<KestrelServerOptions>(options => { options.Limits.MaxRequestBodySize = null; });

builder.Host.UseSerilog();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // TODO: add authorization
    app.MapOpenApi();
}

app.UseStaticFiles();
app.UseRouting();

app.UseSerilogRequestLogging();

app.UseExceptionEndpoint();

app.UseAuthentication();
app.UseAuthorization();

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

app.UseEndpointDefinitions("/api/scriptbee");

// var pluginManager = app.Services.GetRequiredService<PluginManager>();

// todo move to task
// pluginManager.LoadPlugins();

app.Run();

public partial class Program;

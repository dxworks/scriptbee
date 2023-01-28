using FluentValidation;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using ScriptBee.Marketplace.Client;
using ScriptBee.Plugin;
using ScriptBeeWebApp.EndpointDefinitions.Arguments.Validation;
using ScriptBeeWebApp.Extensions;
using ScriptBeeWebApp.Hubs;
using Serilog;

var builder = WebApplication.CreateBuilder(args);


var mongoConnectionString = builder.Configuration.GetConnectionString("mongodb");
var userFolderConfigurationSection = builder.Configuration.GetSection("UserFolder");

builder.Services
    .AddMongoDb(mongoConnectionString)
    .AddSerilog()
    .AddUtilityServices()
    .AddPluginServices()
    .AddScriptBeeMarketplaceClient()
    .AddFileWatcherServices()
    .AddControllerServices(userFolderConfigurationSection)
    .AddValidatorsFromAssemblyContaining<IValidationMarker>()
    .AddCors(options => options.AddPolicy("CorsPolicy", builder =>
        {
            builder.AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
                .AllowAnyOrigin();
        }
    ))
    .AddSignalR();

builder.Services.AddEndpointDefinitions();

builder.Services.Configure<KestrelServerOptions>(options => { options.Limits.MaxRequestBodySize = null; });

builder.Host.UseSerilog((context, services, config) => config
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services));

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

//todo: add Authentication
// todo: add Authorization
app.UseSerilogRequestLogging();
app.UseExceptionHandler("/error");

// app.UseCors("CorsPolicy");
app.UseEndpoints(_ => { });

app.UseSpa(spa =>
{
    spa.Options.SourcePath = "../ScriptBeeClient";

    if (app.Environment.IsDevelopment())
    {
        spa.UseProxyToSpaDevelopmentServer("http://localhost:4200");
    }
});

app.MapHub<FileWatcherHub>("/api/fileWatcherHub");

app.UseEndpointDefinitions();

var pluginManager = app.Services.GetRequiredService<PluginManager>();

// todo move to task
pluginManager.LoadPlugins();

app.Run();

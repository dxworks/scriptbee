using FluentValidation;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using ScriptBee.Analysis.Web.EndpointDefinitions;
using ScriptBee.Analysis.Web.Extensions;
using ScriptBee.Artifacts.Extensions;
using ScriptBee.Common.Web;
using ScriptBee.Common.Web.EndpointDefinition;
using ScriptBee.Common.Web.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

var mongoConnectionString = builder.Configuration.GetConnectionString("mongodb");
var scriptBeeConfigurationSection = builder.Configuration.GetSection("ScriptBee");

builder
    .Services.AddConfiguredHealthChecks()
    .AddSerilog()
    .AddOpenApi(options =>
    {
        options.AddDocumentTransformer(
            (document, _, _) =>
            {
                document.Info.Title = "ScriptBee Analysis API";
                document.Info.Version = "v2";
                document.Info.Description =
                    "API for ScriptBee Analysis service, responsible for running scripts and processing data.";
                return Task.CompletedTask;
            }
        );

        options.StripWebPrefix();
        options.AddDescriptionSupport();
    })
    .AddValidatorsFromAssemblyContaining<IEndpointDefinitionMarker>()
    .AddProblemDetailsDefaults()
    .AddCommonServices()
    .AddInstanceConfig(scriptBeeConfigurationSection)
    .AddMongoDb(mongoConnectionString)
    .AddArtifactFileAdapters()
    .AddPluginServices("ScriptBee:Plugins")
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

app.Run();

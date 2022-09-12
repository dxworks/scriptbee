using System;
using DxWorks.ScriptBee.Plugin.Api;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using ScriptBee.Config;
using ScriptBee.FileManagement;
using ScriptBee.Plugin;
using ScriptBee.ProjectContext;
using ScriptBee.Services;
using ScriptBeeWebApp.Controllers.Arguments.Validation;
using ScriptBeeWebApp.Services;
using Serilog;

namespace ScriptBeeWebApp;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        // services.AddControllersWithViews();

        var mongoConnectionString = Configuration.GetConnectionString("mongodb");

        if (string.IsNullOrEmpty(mongoConnectionString))
        {
            throw new Exception("Mongo Connection String not set");
        }

        var mongoUrl = new MongoUrl(mongoConnectionString);
        var mongoClient = new MongoClient(mongoUrl);
        var mongoDatabase = mongoClient.GetDatabase(mongoUrl.DatabaseName);

        var userFolderPath = Configuration.GetSection("USER_FOLDER_PATH").Value ?? "";

        services.AddSingleton(_ => mongoDatabase);
        services.AddSingleton<ILogger>(_ => new LoggerConfiguration().CreateLogger());
        services.AddSingleton<ILoadersHolder, LoadersHolder>();
        services.AddSingleton<ILinkersHolder, LinkersHolder>();
        services.AddSingleton<IPluginRepository, PluginRepository>();
        services.AddSingleton<IPluginService, PluginService>();
        services.AddSingleton<IProjectManager, ProjectManager>();
        services.AddSingleton<IProjectFileStructureManager, ProjectFileStructureManager>(_ =>
            new ProjectFileStructureManager(userFolderPath));
        services.AddSingleton<IHelperFunctionsFactory, HelperFunctionsFactory>();
        services.AddSingleton<IFileNameGenerator, FileNameGenerator>();
        services.AddSingleton<IFileModelService, FileModelService>();
        services.AddSingleton<IProjectStructureService, ProjectStructureService>();
        services.AddSingleton<IProjectModelService, ProjectModelService>();
        services.AddSingleton<IRunModelService, RunModelService>();
        services.AddSingleton<IPluginDiscriminatorHolder, PluginDiscriminatorHolder>();
        services.AddSingleton<IPluginManifestYamlFileReader, PluginManifestYamlFileReader>();
        services.AddSingleton<IGenerateScriptService, GenerateScriptService>();
        services.AddSingleton<IFileService, FileService>();
        services.AddSingleton<IFileModelService, FileModelService>();
        services.AddSingleton<IPluginManifestValidator, PluginManifestValidator>();
        services.AddSingleton<IPluginReader, PluginReader>();
        services.AddSingleton<IDllLoader, DllLoader>();
        services.AddSingleton<IPluginLoaderFactory, PluginLoaderFactory>();
        services.AddSingleton<PluginManager>();

        // add Plugin Loaders
        // check if services.TryAddEnumerable is needed
        services.AddSingleton<IPluginLoader, ScriptGeneratorPluginLoader>();
        services.AddSingleton<IPluginLoader, UiPluginLoader>();
        services.AddSingleton<IPluginLoader, LinkerPluginLoader>();
        services.AddSingleton<IPluginLoader, LoaderPluginLoader>();

        services.AddValidatorsFromAssemblyContaining<IValidationMarker>();

        services.AddControllers();
        services.AddSwaggerGen();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            // app.UseHsts();
        }

        // app.UseHttpsRedirection();
        // app.UseStaticFiles();

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller}/{action=Index}/{id?}");
        });

        var pluginManager = app.ApplicationServices.GetRequiredService<PluginManager>();

        // todo move to task
        pluginManager.LoadPlugins(ConfigFolders.PathToPlugins);
    }
}

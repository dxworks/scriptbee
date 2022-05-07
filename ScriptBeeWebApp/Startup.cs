using System;
using System.Linq;
using System.Reflection;
using HelperFunctions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using ScriptBee.Config;
using ScriptBee.PluginManager;
using ScriptBee.ProjectContext;
using ScriptBee.Scripts.ScriptSampleGenerators.Strategies;
using ScriptBeePlugin;
using ScriptBeeWebApp.FolderManager;
using ScriptBeeWebApp.Services;

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

        services.AddSingleton(_ => mongoDatabase);
        services.AddSingleton<IFolderWriter, FolderWriter>();
        services.AddSingleton<ILoadersHolder, LoadersHolder>();
        services.AddSingleton<IFileContentProvider, RelativeFileContentProvider>();
        services.AddSingleton<IProjectManager, ProjectManager>();
        services.AddSingleton<IProjectFileStructureManager, ProjectFileStructureManager>();
        services.AddSingleton<IHelperFunctionsFactory, HelperFunctionsFactory>();
        services.AddSingleton<IHelperFunctionsMapper, HelperFunctionsMapper>();
        services.AddSingleton<IFileNameGenerator, FileNameGenerator>();
        services.AddSingleton<IFileModelService, FileModelService>();
        services.AddSingleton<IProjectStructureService, ProjectStructureService>();
        services.AddSingleton<IProjectModelService, ProjectModelService>();
        services.AddSingleton<IRunModelService, RunModelService>();
        services.AddSingleton<IFileModelService, FileModelService>();

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

        var loadersHolder = (ILoadersHolder)app.ApplicationServices.GetService(typeof(ILoadersHolder));

        CreateLoadersDictionary(loadersHolder);
    }

    private void CreateLoadersDictionary(ILoadersHolder loadersHolder)
    {
        var pluginPaths = new PluginPathReader(ConfigFolders.PathToPlugins).GetPluginPaths();

        Assembly CurrentDomainOnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            return ((AppDomain)sender).GetAssemblies().FirstOrDefault(assembly => assembly.FullName == args.Name);
        }

        AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainOnAssemblyResolve;

        foreach (var pluginPath in pluginPaths)
        {
            var pluginDll = Assembly.LoadFrom(pluginPath);

            foreach (var type in pluginDll.GetExportedTypes())
            {
                if (typeof(IModelLoader).IsAssignableFrom(type))
                {
                    var modelLoader = (IModelLoader)Activator.CreateInstance(type);
                    loadersHolder.AddLoaderToDictionary(modelLoader);
                }
            }
        }

        AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomainOnAssemblyResolve;
    }
}
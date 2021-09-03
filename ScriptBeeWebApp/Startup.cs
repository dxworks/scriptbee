using System;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ScriptBee.PluginManager;
using ScriptBee.Scripts.ScriptSampleGenerators.Strategies;
using ScriptBeePlugin;
using ScriptBeeWebApp.Config;
using ScriptBeeWebApp.FolderManager;

namespace ScriptBeeWebApp
{
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
            services.AddControllersWithViews();
            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration => { configuration.RootPath = "ClientApp/dist"; });
            services.AddSingleton<IFolderWriter, FolderWriter>();
            services.AddSingleton<ILoadersHolder, LoadersHolder>();
            services.AddSingleton<IFileContentProvider, FileContentProvider>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                     spa.UseAngularCliServer(npmScript: "start");
                    // spa.UseProxyToSpaDevelopmentServer("http://localhost:4200");
                }
            });
            
            var loadersHolder = (ILoadersHolder) app.ApplicationServices.GetService(typeof(ILoadersHolder));

            CreateLoadersDictionary(loadersHolder);
        }

        private void CreateLoadersDictionary(ILoadersHolder loadersHolder)
        {
            var pluginPaths = new PluginPathReader(ConfigFolders.PathToPlugins).GetPluginPaths();

            foreach (var pluginPath in pluginPaths)
            {
                var pluginDLL = Assembly.LoadFile(pluginPath);

                foreach(Type type in pluginDLL.GetExportedTypes())
                {
                    if (typeof(IModelLoader).IsAssignableFrom(type))
                    {
                        var modelLoader = (IModelLoader) Activator.CreateInstance(type);
                        loadersHolder.AddLoaderToDictionary(modelLoader);
                    }
                }
            }
        }
    }
}
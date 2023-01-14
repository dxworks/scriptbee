using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ScriptBee.Marketplace.Client;
using ScriptBee.Plugin;
using ScriptBeeWebApp.Controllers.Arguments.Validation;
using ScriptBeeWebApp.Extensions;
using ScriptBeeWebApp.Hubs;

namespace ScriptBeeWebApp;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        var mongoConnectionString = Configuration.GetConnectionString("mongodb");
        var userFolderConfigurationSection = Configuration.GetSection("UserFolder");

        services
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
        }

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapHub<FileWatcherHub>("/api/fileWatcherHub");

            endpoints.MapControllerRoute(
                "default",
                "{controller}/{action=Index}/{id?}");
        });

        var pluginManager = app.ApplicationServices.GetRequiredService<PluginManager>();

        // todo move to task
        pluginManager.LoadPlugins();
    }
}

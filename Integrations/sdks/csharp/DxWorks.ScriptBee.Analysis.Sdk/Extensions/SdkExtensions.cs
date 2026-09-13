using DxWorks.ScriptBee.Analysis.Sdk.Configuration;
using DxWorks.ScriptBee.Analysis.Sdk.Context;
using DxWorks.ScriptBee.Analysis.Sdk.Endpoints;
using DxWorks.ScriptBee.Analysis.Sdk.Model;
using DxWorks.ScriptBee.Analysis.Sdk.Results;
using DxWorks.ScriptBee.Analysis.Sdk.Scripts;
using DxWorks.ScriptBee.Analysis.Sdk.State;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ScriptBee.Analysis.Mongodb.Extensions;
using ScriptBee.Artifacts.Extensions;
using ScriptBee.Artifacts.Mongodb.Extensions;
using ScriptBee.Common.Web;
using ScriptBee.Common.Web.Extensions;
using ScriptBee.Persistence.Mongodb.Extensions;

namespace DxWorks.ScriptBee.Analysis.Sdk.Extensions;

public static class SdkExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddAnalysisSdk(ConfigurationManager builderConfiguration)
        {
            var mongoConnectionString = builderConfiguration.GetConnectionString("mongodb");
            var scriptBeeConfigurationSection = builderConfiguration.GetSection("ScriptBee");

            return services
                .AddServices()
                .AddAnalysisEndpoints()
                .AddAnalysisMongoDb(mongoConnectionString)
                .AddAnalysisInstanceContext(scriptBeeConfigurationSection)
                .AddArtifactFileAdapters();
        }

        private IServiceCollection AddServices()
        {
            return services
                .AddSingleton<IScriptLoader, ScriptLoader>()
                .AddSingleton<IAnalysisState, AnalysisState>()
                .AddSingleton<IScriptResultsStore, ScriptResultsStore>()
                .AddSingleton<IModelFileLoader, ModelFileLoader>()
                .AddSingleton<FileBundler>();
        }

        private IServiceCollection AddAnalysisMongoDb(string? connectionString)
        {
            var mongoDatabase = services.AddMongodbDatabase(connectionString);

            return services.AddAnalysisAdapters(mongoDatabase).AddScriptAdapters(mongoDatabase);
        }

        private IServiceCollection AddAnalysisEndpoints()
        {
            services.AddValidatorsFromAssemblyContaining<IEndpointDefinitionMarker>();
            services.AddEndpointDefinitions(
                typeof(IEndpointDefinition),
                typeof(IEndpointDefinitionMarker)
            );

            return services;
        }

        private IServiceCollection AddAnalysisInstanceContext(
            IConfigurationSection configurationSection
        )
        {
            services.Configure<AnalysisInstanceOptions>(configurationSection);
            services.AddSingleton<IAnalysisInstanceContext, AnalysisInstanceContext>();

            return services;
        }
    }
}

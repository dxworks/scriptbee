﻿using MongoDB.Driver;
using ScriptBee.Gateway.Persistence.Mongodb;
using ScriptBee.Gateway.Persistence.Mongodb.Contracts;
using ScriptBee.Gateway.Persistence.Mongodb.Exceptions;
using ScriptBee.Gateway.Persistence.Mongodb.Repository;
using ScriptBee.Ports.Driven.Calculation;
using ScriptBee.Ports.Driven.Project;

namespace ScriptBee.Gateway.Web.Extensions;

public static class MongoDbExtensions
{
    public static IServiceCollection AddMongoDb(this IServiceCollection services, string? connectionString)
    {
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidMongoConfigurationException();
        }

        var mongoUrl = new MongoUrl(connectionString);
        var mongoClient = new MongoClient(mongoUrl);
        var mongoDatabase = mongoClient.GetDatabase(mongoUrl.DatabaseName);

        return services.AddSingleton<IMongoClient>(mongoClient)
            .AddAdapters(mongoDatabase);
    }

    private static IServiceCollection AddAdapters(this IServiceCollection services, IMongoDatabase mongoDatabase)
    {
        return services
            .AddProjectAdapters(mongoDatabase)
            .AddProjectInstancesAdapters(mongoDatabase);
    }

    private static IServiceCollection AddProjectAdapters(this IServiceCollection services, IMongoDatabase mongoDatabase)
    {
        return services
            .AddSingleton<IMongoCollection<ProjectModel>>(
                _ => mongoDatabase.GetCollection<ProjectModel>("Projects"))
            .AddSingleton<IMongoRepository<ProjectModel>, MongoRepository<ProjectModel>>()
            .AddSingleton<ICreateProject, ProjectPersistenceAdapter>()
            .AddSingleton<IDeleteProject, ProjectPersistenceAdapter>()
            .AddSingleton<IGetAllProjects, ProjectPersistenceAdapter>()
            .AddSingleton<IGetProject, ProjectPersistenceAdapter>();
    }

    private static IServiceCollection AddProjectInstancesAdapters(this IServiceCollection services,
        IMongoDatabase mongoDatabase)
    {
        return services
            .AddSingleton<IMongoCollection<ProjectInstance>>(
                _ => mongoDatabase.GetCollection<ProjectInstance>("Instances"))
            .AddSingleton<IMongoRepository<ProjectInstance>, MongoRepository<ProjectInstance>>()
            .AddSingleton<IGetAllProjectInstances, ProjectInstancesPersistenceAdapter>();
    }
}

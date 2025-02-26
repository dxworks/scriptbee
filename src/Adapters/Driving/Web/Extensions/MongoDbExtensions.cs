﻿using MongoDB.Driver;
using ScriptBee.Analysis.Ports;
using ScriptBee.Persistence.Mongodb;
using ScriptBee.Persistence.Mongodb.Contracts;
using ScriptBee.Persistence.Mongodb.Exceptions;
using ScriptBee.Persistence.Mongodb.Repository;
using ScriptBee.Project.Ports;

namespace ScriptBee.Web.Extensions;

public static class MongoDbExtensions
{
    public static IServiceCollection AddMongoDb(
        this IServiceCollection services,
        string? connectionString
    )
    {
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidMongoConfigurationException();
        }

        var mongoUrl = new MongoUrl(connectionString);
        var mongoClient = new MongoClient(mongoUrl);
        var mongoDatabase = mongoClient.GetDatabase(mongoUrl.DatabaseName);

        return services.AddSingleton<IMongoClient>(mongoClient).AddAdapters(mongoDatabase);
    }

    private static IServiceCollection AddAdapters(
        this IServiceCollection services,
        IMongoDatabase mongoDatabase
    )
    {
        return services
            .AddProjectAdapters(mongoDatabase)
            .AddProjectInstancesAdapters(mongoDatabase);
    }

    private static IServiceCollection AddProjectAdapters(
        this IServiceCollection services,
        IMongoDatabase mongoDatabase
    )
    {
        return services
            .AddSingleton<IMongoCollection<ProjectModel>>(_ =>
                mongoDatabase.GetCollection<ProjectModel>("Projects")
            )
            .AddSingleton<IMongoRepository<ProjectModel>, MongoRepository<ProjectModel>>()
            .AddSingleton<ICreateProject, ProjectPersistenceAdapter>()
            .AddSingleton<IDeleteProject, ProjectPersistenceAdapter>()
            .AddSingleton<IGetAllProjects, ProjectPersistenceAdapter>()
            .AddSingleton<IGetProject, ProjectPersistenceAdapter>();
    }

    private static IServiceCollection AddProjectInstancesAdapters(
        this IServiceCollection services,
        IMongoDatabase mongoDatabase
    )
    {
        return services
            .AddSingleton<IMongoCollection<ProjectInstance>>(_ =>
                mongoDatabase.GetCollection<ProjectInstance>("Instances")
            )
            .AddSingleton<IMongoRepository<ProjectInstance>, MongoRepository<ProjectInstance>>()
            .AddSingleton<ICreateProjectInstance, ProjectInstancesPersistenceAdapter>()
            .AddSingleton<IGetAllProjectInstances, ProjectInstancesPersistenceAdapter>();
    }
}

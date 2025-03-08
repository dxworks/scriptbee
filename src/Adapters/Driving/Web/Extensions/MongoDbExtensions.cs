﻿using MongoDB.Driver;
using ScriptBee.Persistence.Mongodb;
using ScriptBee.Persistence.Mongodb.Entity;
using ScriptBee.Persistence.Mongodb.Exceptions;
using ScriptBee.Persistence.Mongodb.Repository;
using ScriptBee.Ports.Instance;
using ScriptBee.Ports.Project;

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
            .AddSingleton<IMongoCollection<MongodbProjectModel>>(_ =>
                mongoDatabase.GetCollection<MongodbProjectModel>("Projects")
            )
            .AddSingleton<
                IMongoRepository<MongodbProjectModel>,
                MongoRepository<MongodbProjectModel>
            >()
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
            .AddSingleton<IMongoCollection<MongodbProjectInstance>>(_ =>
                mongoDatabase.GetCollection<MongodbProjectInstance>("Instances")
            )
            .AddSingleton<
                IMongoRepository<MongodbProjectInstance>,
                MongoRepository<MongodbProjectInstance>
            >()
            .AddSingleton<ICreateProjectInstance, ProjectInstancesPersistenceAdapter>()
            .AddSingleton<IGetAllProjectInstances, ProjectInstancesPersistenceAdapter>();
    }
}

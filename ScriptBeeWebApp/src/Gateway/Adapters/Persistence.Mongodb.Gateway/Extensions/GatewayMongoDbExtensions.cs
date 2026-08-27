using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using ScriptBee.Persistence.Mongodb.Entity;
using ScriptBee.Persistence.Mongodb.Indexes;
using ScriptBee.Ports.Instance;
using ScriptBee.Ports.Permissions;
using ScriptBee.Ports.Project;

namespace ScriptBee.Persistence.Mongodb.Extensions;

public static class GatewayMongoDbExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddProjectAdapters(IMongoDatabase mongoDatabase)
        {
            return services
                .AddMongoCollection<MongodbProjectModel>(mongoDatabase, "Projects")
                .AddSingleton<ICreateProject, ProjectPersistenceAdapter>()
                .AddSingleton<IDeleteProject, ProjectPersistenceAdapter>()
                .AddSingleton<IGetAllProjects, ProjectPersistenceAdapter>()
                .AddSingleton<IGetProject, ProjectPersistenceAdapter>()
                .AddSingleton<IUpdateProject, ProjectPersistenceAdapter>();
        }

        public IServiceCollection AddProjectInstancesAdapters(IMongoDatabase mongoDatabase)
        {
            return services
                .AddMongoCollection<MongodbProjectInstance>(mongoDatabase, "Instances")
                .AddSingleton<ICreateProjectInstance, ProjectInstancesPersistenceAdapter>()
                .AddSingleton<IDeleteProjectInstance, ProjectInstancesPersistenceAdapter>()
                .AddSingleton<IGetAllProjectInstances, ProjectInstancesPersistenceAdapter>()
                .AddSingleton<IGetProjectInstance, ProjectInstancesPersistenceAdapter>();
        }

        public IServiceCollection AddResourceMembersAdapters(IMongoDatabase mongoDatabase)
        {
            return services
                .AddMongoCollection<MongodbResourceMember>(mongoDatabase, "ResourceMembers")
                .AddSingleton<IGetResourceRole, ResourceMembersPersistenceAdapter>()
                .AddSingleton<ISetResourceRole, ResourceMembersPersistenceAdapter>()
                .AddSingleton<IGetProjectMembers, ResourceMembersPersistenceAdapter>()
                .AddSingleton<IRemoveProjectMember, ResourceMembersPersistenceAdapter>();
        }

        public IServiceCollection AddUserManagementAdaptersAdapters(IMongoDatabase mongoDatabase)
        {
            return services
                .AddMongoCollection<MongodbUser>(mongoDatabase, "Users")
                .AddSingleton<IIndexCreator, MongodbUserIndexes>()
                .AddSingleton<IIndexCreator, MongodbResourceMemberIndexes>()
                .AddSingleton<IGetOrAddUser, UserManagementPersistenceAdapter>()
                .AddSingleton<IGetAllUsers, UserManagementPersistenceAdapter>();
        }
    }
}

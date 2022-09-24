using Microsoft.EntityFrameworkCore;

using Skoruba.Duende.IdentityServer.Admin.EntityFramework;
using Skoruba.Duende.IdentityServer.Admin.EntityFramework.Repositories;

namespace Microsoft.Extensions.DependencyInjection;

public static class DbExtensions
{
    public static IServiceCollection AddAdminRepositories<TAdminDbContext>(
        this IServiceCollection services)
        where TAdminDbContext : DbContext, IAdminPersistedGrantDbContext, IAdminConfigurationDbContext, IAdminLogDbContext
    {
        return services.AddAdminRepositories<TAdminDbContext, TAdminDbContext, TAdminDbContext>();
    }

    public static IServiceCollection AddAdminRepositories<TConfigurationDbContext, TPersistedGrantDbContext, TLogDbContext>(this IServiceCollection services)
        where TPersistedGrantDbContext : DbContext, IAdminPersistedGrantDbContext
        where TConfigurationDbContext : DbContext, IAdminConfigurationDbContext
        where TLogDbContext : DbContext, IAdminLogDbContext
    {
        //Repositories
        services.AddTransient<IClientRepository, ClientRepository<TConfigurationDbContext>>();
        services.AddTransient<IIdentityResourceRepository, IdentityResourceRepository<TConfigurationDbContext>>();
        services.AddTransient<IApiResourceRepository, ApiResourceRepository<TConfigurationDbContext>>();
        services.AddTransient<IApiScopeRepository, ApiScopeRepository<TConfigurationDbContext>>();
        services.AddTransient<IPersistedGrantRepository, PersistedGrantRepository<TPersistedGrantDbContext>>();
        services.AddTransient<IIdentityProviderRepository, IdentityProviderRepository<TConfigurationDbContext>>();
        services.AddTransient<IKeyRepository, KeyRepository<TPersistedGrantDbContext>>();
        services.AddTransient<ILogRepository, LogRepository<TLogDbContext>>();
        return services;
    }
}

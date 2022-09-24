// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using Microsoft.EntityFrameworkCore;

using Skoruba.AuditLogging.EntityFramework.Entities;
using Skoruba.Duende.IdentityServer.Admin.BusinessLogic.Resources;
using Skoruba.Duende.IdentityServer.Admin.BusinessLogic.Services;
using Skoruba.Duende.IdentityServer.Admin.BusinessLogic.Services.Interfaces;
using Skoruba.Duende.IdentityServer.Admin.EntityFramework;

namespace Microsoft.Extensions.DependencyInjection;

public static class AdminServicesExtensions
{
    public static IServiceCollection AddAdminServices<TAdminDbContext>(
        this IServiceCollection services)
        where TAdminDbContext : DbContext, IAdminPersistedGrantDbContext, IAdminConfigurationDbContext, IAdminLogDbContext
    {
        return services.AddAdminServices<TAdminDbContext, TAdminDbContext, TAdminDbContext>();
    }

    public static IServiceCollection AddAdminServices<TConfigurationDbContext, TPersistedGrantDbContext, TLogDbContext>(this IServiceCollection services)
        where TPersistedGrantDbContext : DbContext, IAdminPersistedGrantDbContext
        where TConfigurationDbContext : DbContext, IAdminConfigurationDbContext
        where TLogDbContext : DbContext, IAdminLogDbContext
    {
        //Repositories
        services.AddAdminRepositories<TConfigurationDbContext, TPersistedGrantDbContext, TLogDbContext>();

        //Services
        services.AddTransient<IClientService, ClientService>();
        services.AddTransient<IApiResourceService, ApiResourceService>();
        services.AddTransient<IApiScopeService, ApiScopeService>();
        services.AddTransient<IIdentityResourceService, IdentityResourceService>();
        services.AddTransient<IPersistedGrantService, PersistedGrantService>();
        services.AddTransient<IIdentityProviderService, IdentityProviderService>();
        services.AddTransient<IKeyService, KeyService>();
        services.AddTransient<ILogService, LogService>();

        //Resources
        services.AddScoped<IApiResourceServiceResources, ApiResourceServiceResources>();
        services.AddScoped<IApiScopeServiceResources, ApiScopeServiceResources>();
        services.AddScoped<IClientServiceResources, ClientServiceResources>();
        services.AddScoped<IIdentityResourceServiceResources, IdentityResourceServiceResources>();
        services.AddScoped<IPersistedGrantServiceResources, PersistedGrantServiceResources>();
        services.AddScoped<IIdentityProviderServiceResources, IdentityProviderServiceResources>();
        services.AddScoped<IKeyServiceResources, KeyServiceResources>();

        return services;
    }

    /// <summary>
    /// Requiring <see cref="IAuditLogRepository"/> be registered.
    /// </summary>
    public static IServiceCollection AddAuditLog<TAuditLog>(this IServiceCollection services) where TAuditLog : AuditLog
    {
        services.AddTransient<IAuditLogService, AuditLogService<TAuditLog>>();
        return services;
    }
}
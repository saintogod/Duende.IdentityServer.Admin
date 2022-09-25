﻿// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System.Reflection;

using Duende.IdentityServer.EntityFramework.Storage;

using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using Skoruba.AuditLogging.EntityFramework.DbContexts;
using Skoruba.AuditLogging.EntityFramework.Entities;
using Skoruba.Duende.IdentityServer.Admin.EntityFramework.Configuration.Configuration;
using Skoruba.Duende.IdentityServer.Admin.EntityFramework;

namespace Microsoft.Extensions.DependencyInjection;

public static partial class DatabaseExtensions
{
    /// <summary>
    /// Register DbContexts for IdentityServer ConfigurationStore and PersistedGrants, Identity and Logging
    /// Configure the connection strings in AppSettings.json
    /// </summary>
    public static IServiceCollection RegisterMySqlDbContexts<TIdentityDbContext, TConfigurationDbContext,
        TPersistedGrantDbContext, TLogDbContext, TAuditLoggingDbContext, TDataProtectionDbContext, TAuditLog>(this IServiceCollection services,
        ConnectionStringsConfiguration connectionStrings,
        DatabaseMigrationsConfiguration databaseMigrations)
        where TIdentityDbContext : DbContext
        where TPersistedGrantDbContext : DbContext, IAdminPersistedGrantDbContext
        where TConfigurationDbContext : DbContext, IAdminConfigurationDbContext
        where TLogDbContext : DbContext, IAdminLogDbContext
        where TAuditLoggingDbContext : DbContext, IAuditLoggingDbContext<TAuditLog>
        where TDataProtectionDbContext : DbContext, IDataProtectionKeyContext
        where TAuditLog : AuditLog
    {
        var migrationsAssembly = typeof(DatabaseExtensions).GetTypeInfo().Assembly.GetName().Name;

        // Config DB for identity
        services.AddDbContext<TIdentityDbContext>(options =>
        {
            options.UseMySql(connectionStrings.IdentityDbConnection,
                ServerVersion.AutoDetect(connectionStrings.IdentityDbConnection),
                    sql => sql.MigrationsAssembly(databaseMigrations.IdentityDbMigrationsAssembly ?? migrationsAssembly));
        });

        // Config DB from existing connection
        services.AddConfigurationDbContext<TConfigurationDbContext>(options =>
        {
            options.ConfigureDbContext = b => b.UseMySql(connectionStrings.ConfigurationDbConnection,
                    ServerVersion.AutoDetect(connectionStrings.ConfigurationDbConnection),
                        sql => sql.MigrationsAssembly(databaseMigrations.ConfigurationDbMigrationsAssembly ?? migrationsAssembly));
        });

        // Operational DB from existing connection
        services.AddOperationalDbContext<TPersistedGrantDbContext>(options =>
        {
            options.ConfigureDbContext = b =>
                b.UseMySql(connectionStrings.PersistedGrantDbConnection, ServerVersion.AutoDetect(connectionStrings.PersistedGrantDbConnection),
                sql => sql.MigrationsAssembly(databaseMigrations.PersistedGrantDbMigrationsAssembly ?? migrationsAssembly));
        });

        // Log DB from existing connection
        services.AddDbContext<TLogDbContext>(options =>
        {
            options.UseMySql(connectionStrings.AdminLogDbConnection, ServerVersion.AutoDetect(connectionStrings.AdminLogDbConnection),
            optionsSql => optionsSql.MigrationsAssembly(databaseMigrations.AdminLogDbMigrationsAssembly ?? migrationsAssembly));
        });

        // Audit logging connection
        services.AddDbContext<TAuditLoggingDbContext>(options =>
        {
            options.UseMySql(connectionStrings.AdminAuditLogDbConnection, ServerVersion.AutoDetect(connectionStrings.AdminAuditLogDbConnection),
            optionsSql => optionsSql.MigrationsAssembly(databaseMigrations.AdminAuditLogDbMigrationsAssembly ?? migrationsAssembly));
        });

        // DataProtectionKey DB from existing connection
        if (!string.IsNullOrEmpty(connectionStrings.DataProtectionDbConnection))
        {
            services.AddDbContext<TDataProtectionDbContext>(options =>
            {
                options.UseMySql(connectionStrings.DataProtectionDbConnection, ServerVersion.AutoDetect(connectionStrings.DataProtectionDbConnection),
                    optionsSql => optionsSql.MigrationsAssembly(databaseMigrations.DataProtectionDbMigrationsAssembly ?? migrationsAssembly));
            });
        }
        return services;
    }

    /// <summary>
    /// Register DbContexts for IdentityServer ConfigurationStore and PersistedGrants and Identity
    /// Configure the connection strings in AppSettings.json
    /// </summary>
    public static IServiceCollection RegisterMySqlDbContexts<TIdentityDbContext, TConfigurationDbContext,
        TPersistedGrantDbContext, TDataProtectionDbContext>(this IServiceCollection services,
        string identityConnectionString, string configurationConnectionString,
        string persistedGrantConnectionString, string dataProtectionConnectionString)
        where TIdentityDbContext : DbContext
        where TPersistedGrantDbContext : DbContext, IAdminPersistedGrantDbContext
        where TConfigurationDbContext : DbContext, IAdminConfigurationDbContext
        where TDataProtectionDbContext : DbContext, IDataProtectionKeyContext
    {
        var migrationsAssembly = typeof(DatabaseExtensions).GetTypeInfo().Assembly.GetName().Name;

        // Config DB for identity
        services.AddDbContext<TIdentityDbContext>(options =>
        {
            options.UseMySql(identityConnectionString, ServerVersion.AutoDetect(identityConnectionString), sql => sql.MigrationsAssembly(migrationsAssembly));
        });

        // Config DB from existing connection
        services.AddConfigurationDbContext<TConfigurationDbContext>(options =>
        {
            options.ConfigureDbContext = b => b.UseMySql(configurationConnectionString, ServerVersion.AutoDetect(configurationConnectionString), sql => sql.MigrationsAssembly(migrationsAssembly));
        });

        // Operational DB from existing connection
        services.AddOperationalDbContext<TPersistedGrantDbContext>(options =>
        {
            options.ConfigureDbContext = b => b.UseMySql(persistedGrantConnectionString, ServerVersion.AutoDetect(persistedGrantConnectionString), sql => sql.MigrationsAssembly(migrationsAssembly));
        });

        // DataProtectionKey DB from existing connection
        services.AddDbContext<TDataProtectionDbContext>(options =>
        {
            options.UseMySql(dataProtectionConnectionString, ServerVersion.AutoDetect(dataProtectionConnectionString), optionsSql => optionsSql.MigrationsAssembly(migrationsAssembly));
        });
        return services;
    }
}
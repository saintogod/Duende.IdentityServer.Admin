﻿// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Skoruba.Duende.IdentityServer.Admin.EntityFramework.Configuration.Configuration;
using Skoruba.Duende.IdentityServer.Admin.UI.Configuration;

namespace Skoruba.Duende.IdentityServer.Admin.UI.Helpers.DependencyInjection;

public class IdentityServerAdminUIOptions
{
    /// <summary>
    /// The settings for test deployments.
    /// </summary>
    public TestingConfiguration Testing { get; set; } = new();

    /// <summary>
    /// The database connection strings and settings.
    /// </summary>
    public ConnectionStringsConfiguration ConnectionStrings { get; set; } = new();

    /// <summary>
    /// The settings for the admin services.
    /// </summary>
    public AdminConfiguration Admin { get; set; } = new();

    /// <summary>
    /// The settings for the database provider.
    /// </summary>
    public DatabaseProviderConfiguration DatabaseProvider { get; set; } = new();

    /// <summary>
    /// The settings for database migrations.
    /// </summary>
    public DatabaseMigrationsConfiguration DatabaseMigrations { get; set; } = new();

    /// <summary>
    /// The settings for audit logging.
    /// </summary>
    public AuditLoggingConfiguration AuditLogging { get; set; } = new();

    /// <summary>
    /// The settings for globalization.
    /// </summary>
    public CultureConfiguration Culture { get; set; } = new();

    /// <summary>
    /// An action to configure ASP.NET Core Identity.
    /// </summary>
    public Action<IdentityOptions> IdentityConfigureAction { get; set; } = options => { };

    /// <summary>
    /// Identity data to seed the databases.
    /// </summary>
    public IdentityData IdentityData { get; set; } = new ();

    /// <summary>
    /// Identity server data to seed the databases.
    /// </summary>
    public IdentityServerData IdentityServerData { get; set; } = new ();

    /// <summary>
    /// The settings for security features.
    /// </summary>
    public SecurityConfiguration Security { get; set; } = new ();

    /// <summary>
    /// The settings for the HTTP hosting environment.
    /// </summary>
    public HttpConfiguration Http { get; set; } = new ();

    /// <summary>
    /// Customizes the health checks builder used to add health checks.
    /// </summary>
    public Func<IServiceCollection, IHealthChecksBuilder> HealthChecksBuilderFactory { get; set; }

    /// <summary>
    /// Applies configuration parsed from an appsettings file into these options.
    /// </summary>
    /// <param name="configuration">The configuration to bind into this instance.</param>
    public void BindConfiguration(IConfiguration configuration)
    {
        configuration.GetSection(nameof(TestingConfiguration)).Bind(Testing);
        configuration.GetSection(ConfigurationConsts.ConnectionStringsKey).Bind(ConnectionStrings);
        configuration.GetSection(nameof(AdminConfiguration)).Bind(Admin);
        configuration.GetSection(nameof(DatabaseProviderConfiguration)).Bind(DatabaseProvider);
        configuration.GetSection(nameof(DatabaseMigrationsConfiguration)).Bind(DatabaseMigrations);
        configuration.GetSection(nameof(AuditLoggingConfiguration)).Bind(AuditLogging);
        configuration.GetSection(nameof(CultureConfiguration)).Bind(Culture);
        IdentityConfigureAction = options => configuration.GetSection(nameof(IdentityOptions)).Bind(options);
        configuration.GetSection(nameof(SecurityConfiguration)).Bind(Security);
        configuration.GetSection(nameof(HttpConfiguration)).Bind(Http);
        configuration.GetSection(nameof(IdentityServerData)).Bind(IdentityServerData);
        configuration.GetSection(nameof(IdentityData)).Bind(IdentityData);
    }
}

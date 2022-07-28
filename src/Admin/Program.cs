// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using Serilog;

using Skoruba.Duende.IdentityServer.Admin;
using Skoruba.Duende.IdentityServer.Admin.EntityFramework.Configuration.Configuration;
using Skoruba.Duende.IdentityServer.Admin.EntityFramework.Shared.DbContexts;
using Skoruba.Duende.IdentityServer.Admin.EntityFramework.Shared.Entities.Identity;
using Skoruba.Duende.IdentityServer.Admin.EntityFramework.Shared.Helpers;

const string SeedArgs = "/seed";
const string MigrateOnlyArgs = "/migrateonly";

var configuration = GetConfiguration(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateLogger();

try
{
    var host = CreateHostBuilder(args).Build();

    var migrationComplete = await ApplyDbMigrationsWithDataSeedAsync(args, host);
    if (await MigrateOnlyOperationAsync(args, host, migrationComplete)) return;

    await host.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}


static async Task<bool> MigrateOnlyOperationAsync(string[] args, IHost host, bool migrationComplete)
{
    if (args.Any(x => x == MigrateOnlyArgs))
    {
        await host.StopAsync();

        if (!migrationComplete)
        {
            Environment.ExitCode = -1;
        }

        return true;
    }

    return false;
}

static async Task<bool> ApplyDbMigrationsWithDataSeedAsync(string[] args, IHost host)
{
    var applyDbMigrationWithDataSeedFromProgramArguments = args.Any(x => x == SeedArgs);

    return await DbMigrationHelpers
        .ApplyDbMigrationsWithDataSeedAsync<IdentityServerConfigurationDbContext, AdminIdentityDbContext,
            IdentityServerPersistedGrantDbContext, AdminLogDbContext, AdminAuditLogDbContext,
            IdentityServerDataProtectionDbContext, UserIdentity, UserIdentityRole>(host,
            applyDbMigrationWithDataSeedFromProgramArguments);
}

static IConfiguration GetConfiguration(string[] args)
{
    var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
    var isDevelopment = environment == Environments.Development;

    var configurationBuilder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
        .AddJsonFile("serilog.json", optional: true, reloadOnChange: true)
        .AddJsonFile($"serilog.{environment}.json", optional: true, reloadOnChange: true)
        .AddJsonFile("deployment.json", optional: true, reloadOnChange: false);

    if (isDevelopment)
    {
        configurationBuilder.AddUserSecrets<Startup>(true);
    }

    configurationBuilder.AddCommandLine(args);
    configurationBuilder.AddEnvironmentVariables();

    return configurationBuilder.Build();
}

static IHostBuilder CreateHostBuilder(string[] args) =>
   Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration((hostContext, configApp) =>
        {
            configApp.AddJsonFile("serilog.json", optional: true, reloadOnChange: true);
            configApp.AddJsonFile("identitydata.json", optional: true, reloadOnChange: true);
            configApp.AddJsonFile("identityserverdata.json", optional: true, reloadOnChange: true);

            var env = hostContext.HostingEnvironment;

            configApp.AddJsonFile($"serilog.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"identitydata.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"identityserverdata.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
            .AddJsonFile("deployment.json", optional: true, reloadOnChange: false);

            if (env.IsDevelopment())
            {
                configApp.AddUserSecrets<Startup>(true);
            }

            configApp.AddEnvironmentVariables();
            configApp.AddCommandLine(args);
        })
       .ConfigureWebHostDefaults(webBuilder =>
       {
           webBuilder.ConfigureKestrel(options => options.AddServerHeader = false);
           webBuilder.UseStartup<Startup>();
       })
       .UseSerilog((hostContext, loggerConfig) =>
       {
           loggerConfig
               .ReadFrom.Configuration(hostContext.Configuration)
               .Enrich.WithProperty("ApplicationName", hostContext.HostingEnvironment.ApplicationName);
       });

using Skoruba.Duende.IdentityServer.Admin.EntityFramework.Shared.DbContexts;
using Skoruba.Duende.IdentityServer.Admin.EntityFramework.Shared.Entities.Identity;
using Skoruba.Duende.IdentityServer.Admin.EntityFramework.Shared.Helpers;

namespace Skoruba.Duende.IdentityServer.Admin.Services;

internal sealed class DataMigrator : IHostedService
{
    private readonly IServiceProvider serviceProvider;
    private readonly IHostApplicationLifetime lifetime;
    private readonly IConfiguration configuration;
    private readonly ILogger<DataMigrator> logger;

    private bool isRunning;

    public DataMigrator(IServiceProvider serviceProvider, IHostApplicationLifetime lifetime, IConfiguration configuration, ILogger<DataMigrator> logger)
    {
        this.serviceProvider = serviceProvider;
        this.lifetime = lifetime;
        this.configuration = configuration;
        this.logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        isRunning = true;
        var migrationComplete = await DbMigrationHelpers
            .ApplyDbMigrationsWithDataSeedAsync<IdentityServerConfigurationDbContext, AdminIdentityDbContext,
                IdentityServerPersistedGrantDbContext, AdminLogDbContext, AdminAuditLogDbContext,
                IdentityServerDataProtectionDbContext, UserIdentity, UserIdentityRole>(serviceProvider, configuration);

        isRunning = false;
        if (configuration.GetValue("MigrateOnly", false))
        {
            logger.LogInformation("Only need to do the migration tasks.");
            lifetime.StopApplication();

            if (!migrationComplete)
            {
                Environment.ExitCode = -1;
                logger.LogInformation("Migration is broken.");
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        if (isRunning)
        {
            logger.LogInformation("Migration is in running.");
        }
        return Task.CompletedTask;
    }
}

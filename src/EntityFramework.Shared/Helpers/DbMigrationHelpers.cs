﻿// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System.Security.Claims;

using Duende.IdentityServer.EntityFramework.Mappers;
using Duende.IdentityServer.Models;

using IdentityModel;

using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Skoruba.AuditLogging.EntityFramework.DbContexts;
using Skoruba.AuditLogging.EntityFramework.Entities;
using Skoruba.Duende.IdentityServer.Admin.EntityFramework.Configuration.Configuration;

namespace Skoruba.Duende.IdentityServer.Admin.EntityFramework.Shared.Helpers;

public static class DbMigrationHelpers
{
    /// <summary>
    /// Generate migrations before running this method, you can use these steps bellow:
    /// https://github.com/skoruba/Duende.IdentityServer.Admin#ef-core--data-access
    /// </summary>
    public static async Task<bool> ApplyDbMigrationsWithDataSeedAsync<TIdentityServerDbContext, TIdentityDbContext,
        TPersistedGrantDbContext, TLogDbContext, TAuditLogDbContext, TDataProtectionDbContext, TUser, TRole>(
        IServiceProvider sp, IConfiguration configuration)
        where TIdentityServerDbContext : DbContext, IAdminConfigurationDbContext
        where TIdentityDbContext : DbContext
        where TPersistedGrantDbContext : DbContext, IAdminPersistedGrantDbContext
        where TLogDbContext : DbContext, IAdminLogDbContext
        where TAuditLogDbContext : DbContext, IAuditLoggingDbContext<AuditLog>
        where TDataProtectionDbContext : DbContext, IDataProtectionKeyContext
        where TUser : IdentityUser, new()
        where TRole : IdentityRole, new()
    {
        var migrationComplete = false;
        {
            using var serviceScope = sp.CreateScope();

            if (configuration.GetValue("ApplyMigrations", false))
            {
                migrationComplete = await EnsureDatabasesMigratedAsync<TIdentityDbContext, TIdentityServerDbContext, TPersistedGrantDbContext, TLogDbContext, TAuditLogDbContext, TDataProtectionDbContext>(serviceScope.ServiceProvider);
            }
        }
        {
            using var serviceScope = sp.CreateScope();
            if (configuration.GetValue("ApplySeed", false))
            {
                var seedComplete = await EnsureSeedDataAsync<TIdentityServerDbContext, TUser, TRole>(serviceScope.ServiceProvider);

                return migrationComplete && seedComplete;
            }
        }
        return migrationComplete;
    }

    private static async Task<bool> EnsureDatabasesMigratedAsync<TIdentityDbContext, TConfigurationDbContext, TPersistedGrantDbContext, TLogDbContext, TAuditLogDbContext, TDataProtectionDbContext>(IServiceProvider serviceProvider)
        where TIdentityDbContext : DbContext
        where TPersistedGrantDbContext : DbContext
        where TConfigurationDbContext : DbContext
        where TLogDbContext : DbContext
        where TAuditLogDbContext : DbContext
        where TDataProtectionDbContext : DbContext
    {
        var pendingMigrationCount = 0;

        using (var context = serviceProvider.GetRequiredService<TPersistedGrantDbContext>())
        {
            await context.Database.MigrateAsync();
            pendingMigrationCount += (await context.Database.GetPendingMigrationsAsync()).Count();
        }

        using (var context = serviceProvider.GetRequiredService<TIdentityDbContext>())
        {
            await context.Database.MigrateAsync();
            pendingMigrationCount += (await context.Database.GetPendingMigrationsAsync()).Count();
        }

        using (var context = serviceProvider.GetRequiredService<TConfigurationDbContext>())
        {
            await context.Database.MigrateAsync();
            pendingMigrationCount += (await context.Database.GetPendingMigrationsAsync()).Count();
        }

        using (var context = serviceProvider.GetRequiredService<TLogDbContext>())
        {
            await context.Database.MigrateAsync();
            pendingMigrationCount += (await context.Database.GetPendingMigrationsAsync()).Count();
        }

        using (var context = serviceProvider.GetRequiredService<TAuditLogDbContext>())
        {
            await context.Database.MigrateAsync();
            pendingMigrationCount += (await context.Database.GetPendingMigrationsAsync()).Count();
        }

        using (var context = serviceProvider.GetRequiredService<TDataProtectionDbContext>())
        {
            await context.Database.MigrateAsync();
            pendingMigrationCount += (await context.Database.GetPendingMigrationsAsync()).Count();
        }

        return pendingMigrationCount == 0;
    }

    private static async Task<bool> EnsureSeedDataAsync<TIdentityServerDbContext, TUser, TRole>(IServiceProvider serviceProvider)
        where TIdentityServerDbContext : DbContext, IAdminConfigurationDbContext
        where TUser : IdentityUser, new()
        where TRole : IdentityRole, new()
    {
        var context = serviceProvider.GetRequiredService<TIdentityServerDbContext>();
        var idsDataConfiguration = serviceProvider.GetRequiredService<IdentityServerData>();

        var userManager = serviceProvider.GetRequiredService<UserManager<TUser>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<TRole>>();
        var idDataConfiguration = serviceProvider.GetRequiredService<IdentityData>();

        await EnsureSeedIdentityServerData(context, idsDataConfiguration);
        await EnsureSeedIdentityData(userManager, roleManager, idDataConfiguration);

        return true;
    }

    /// <summary>
    /// Generate default admin user / role
    /// </summary>
    private static async Task EnsureSeedIdentityData<TUser, TRole>(UserManager<TUser> userManager,
        RoleManager<TRole> roleManager, IdentityData identityDataConfiguration)
        where TUser : IdentityUser, new()
        where TRole : IdentityRole, new()
    {
        // adding roles from seed
        foreach (var r in identityDataConfiguration.Roles)
        {
            if (!await roleManager.RoleExistsAsync(r.Name))
            {
                var role = new TRole
                {
                    Name = r.Name
                };

                var result = await roleManager.CreateAsync(role);

                if (result.Succeeded)
                {
                    foreach (var claim in r.Claims)
                    {
                        await roleManager.AddClaimAsync(role, new Claim(claim.Type, claim.Value));
                    }
                }
            }
        }

        // adding users from seed
        foreach (var user in identityDataConfiguration.Users)
        {
            var identityUser = new TUser
            {
                UserName = user.Username,
                Email = user.Email,
                EmailConfirmed = true
            };

            var userByUserName = await userManager.FindByNameAsync(user.Username);
            var userByEmail = await userManager.FindByEmailAsync(user.Email);

            // User is already exists in database
            if (userByUserName != default || userByEmail != default)
            {
                continue;
            }

            // if there is no password we create user without password
            // user can reset password later, because accounts have EmailConfirmed set to true
            var result = !string.IsNullOrEmpty(user.Password)
            ? await userManager.CreateAsync(identityUser, user.Password)
            : await userManager.CreateAsync(identityUser);

            if (result.Succeeded)
            {
                foreach (var claim in user.Claims)
                {
                    await userManager.AddClaimAsync(identityUser, new Claim(claim.Type, claim.Value));
                }

                foreach (var role in user.Roles)
                {
                    await userManager.AddToRoleAsync(identityUser, role);
                }
            }
        }
    }

    /// <summary>
    /// Generate default clients, identity and api resources
    /// </summary>
    private static async Task EnsureSeedIdentityServerData<TIdentityServerDbContext>(TIdentityServerDbContext context, IdentityServerData identityServerDataConfiguration)
        where TIdentityServerDbContext : DbContext, IAdminConfigurationDbContext
    {
        foreach (var resource in identityServerDataConfiguration.IdentityResources)
        {
            var exits = await context.IdentityResources.AnyAsync(a => a.Name == resource.Name);

            if (exits)
            {
                continue;
            }

            await context.IdentityResources.AddAsync(resource.ToEntity());
        }

        foreach (var apiScope in identityServerDataConfiguration.ApiScopes)
        {
            var exits = await context.ApiScopes.AnyAsync(a => a.Name == apiScope.Name);

            if (exits)
            {
                continue;
            }

            await context.ApiScopes.AddAsync(apiScope.ToEntity());
        }

        foreach (var resource in identityServerDataConfiguration.ApiResources)
        {
            var exits = await context.ApiResources.AnyAsync(a => a.Name == resource.Name);

            if (exits)
            {
                continue;
            }

            foreach (var s in resource.ApiSecrets)
            {
                s.Value = s.Value.ToSha256();
            }

            await context.ApiResources.AddAsync(resource.ToEntity());
        }


        foreach (var client in identityServerDataConfiguration.Clients)
        {
            var exits = await context.Clients.AnyAsync(a => a.ClientId == client.ClientId);

            if (exits)
            {
                continue;
            }

            foreach (var secret in client.ClientSecrets)
            {
                secret.Value = secret.Value.ToSha256();
            }

            client.Claims = client.ClientClaims
                .Select(c => new ClientClaim(c.Type, c.Value))
                .ToList();

            await context.Clients.AddAsync(client.ToEntity());
        }

        await context.SaveChangesAsync();
    }
}

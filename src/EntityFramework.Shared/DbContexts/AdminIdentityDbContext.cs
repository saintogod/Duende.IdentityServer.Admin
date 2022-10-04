﻿// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Skoruba.Duende.IdentityServer.Admin.EntityFramework.Shared.Constants;
using Skoruba.Duende.IdentityServer.Admin.EntityFramework.Shared.Entities.Identity;

namespace Skoruba.Duende.IdentityServer.Admin.EntityFramework.Shared.DbContexts;

public class AdminIdentityDbContext : IdentityDbContext<UserIdentity, UserIdentityRole, string, UserIdentityUserClaim, UserIdentityUserRole, UserIdentityUserLogin, UserIdentityRoleClaim, UserIdentityUserToken>
{
    public AdminIdentityDbContext(DbContextOptions<AdminIdentityDbContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        ConfigureIdentityContext(builder);
    }

    private static void ConfigureIdentityContext(ModelBuilder builder)
    {
        builder.Entity<UserIdentityRole>().ToTable(TableConsts.IdentityRoles);
        builder.Entity<UserIdentityRoleClaim>().ToTable(TableConsts.IdentityRoleClaims);
        builder.Entity<UserIdentityUserRole>().ToTable(TableConsts.IdentityUserRoles);

        builder.Entity<UserIdentity>().ToTable(TableConsts.IdentityUsers);
        builder.Entity<UserIdentityUserLogin>().ToTable(TableConsts.IdentityUserLogins);
        builder.Entity<UserIdentityUserClaim>().ToTable(TableConsts.IdentityUserClaims);
        builder.Entity<UserIdentityUserToken>().ToTable(TableConsts.IdentityUserTokens);
    }
}
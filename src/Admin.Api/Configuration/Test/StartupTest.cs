﻿// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;

using Skoruba.Duende.IdentityServer.Admin.Api.Helpers;
using Skoruba.Duende.IdentityServer.Admin.Api.Middlewares;
using Skoruba.Duende.IdentityServer.Admin.EntityFramework.Shared.DbContexts;
using Skoruba.Duende.IdentityServer.Admin.EntityFramework.Shared.Entities.Identity;

namespace Skoruba.Duende.IdentityServer.Admin.Api.Configuration.Test;

public class StartupTest : Startup
{
    public StartupTest(IWebHostEnvironment env, IConfiguration configuration) : base(env, configuration)
    {
    }

    public override void RegisterDbContexts(IServiceCollection services)
    {
        services.RegisterDbContextsStaging<AdminIdentityDbContext, IdentityServerConfigurationDbContext, IdentityServerPersistedGrantDbContext, AdminLogDbContext, AdminAuditLogDbContext, IdentityServerDataProtectionDbContext>();
    }

    public override void RegisterAuthentication(IServiceCollection services)
    {
        services
            .AddIdentity<UserIdentity, UserIdentityRole>(options => Configuration.GetSection(nameof(IdentityOptions)).Bind(options))
            .AddEntityFrameworkStores<AdminIdentityDbContext>()
            .AddDefaultTokenProviders();

        services.AddAuthentication(options =>
        {
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddCookie(JwtBearerDefaults.AuthenticationScheme);
    }

    public override void RegisterAuthorization(IServiceCollection services)
    {
        services.AddAuthorizationPolicies();
    }

    public override void UseAuthentication(IApplicationBuilder app)
    {
        app.UseAuthentication();
        app.UseMiddleware<AuthenticatedTestRequestMiddleware>();
    }
}
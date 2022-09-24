// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using Skoruba.Duende.IdentityServer.Admin.EntityFramework.Shared.DbContexts;

namespace Skoruba.Duende.IdentityServer.STS.Identity.Configuration.Test;

public class StartupTest : Startup
{
    public StartupTest(IConfiguration configuration) : base(configuration)
    {
    }

    public override void RegisterDbContexts(IServiceCollection services)
    {
        services.RegisterDbContextsStaging<AdminIdentityDbContext, IdentityServerConfigurationDbContext, IdentityServerPersistedGrantDbContext, IdentityServerDataProtectionDbContext>();
    }
}
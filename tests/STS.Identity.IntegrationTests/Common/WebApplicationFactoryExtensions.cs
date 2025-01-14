﻿// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;

using Skoruba.Duende.IdentityServer.STS.Identity.Configuration.Test;

namespace Skoruba.Duende.IdentityServer.STS.Identity.IntegrationTests.Common;

public static class WebApplicationFactoryExtensions
{
    public static HttpClient SetupClient(this WebApplicationFactory<StartupTest> fixture)
    {
        var options = new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        };

        return fixture.WithWebHostBuilder(
            builder => builder
                .UseStartup<StartupTest>()
                .ConfigureTestServices(services => { })
        ).CreateClient(options);
    }
}
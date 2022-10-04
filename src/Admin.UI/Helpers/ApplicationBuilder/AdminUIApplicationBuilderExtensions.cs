// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using HealthChecks.UI.Client;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

using Skoruba.Duende.IdentityServer.Admin.UI.Configuration;

namespace Skoruba.Duende.IdentityServer.Admin.UI.Helpers.ApplicationBuilder;

public static class AdminUIApplicationBuilderExtensions
{
    /// <summary>
    /// Adds the Skoruba Duende IdentityServer Admin UI to the pipeline of this application. This method must be called 
    /// between UseRouting() and UseEndpoints().
    /// </summary>
    public static IApplicationBuilder UseIdentityServerAdminUI(this IApplicationBuilder app)
    {
        return app.UseRoutingDependentMiddleware(app.ApplicationServices.GetRequiredService<TestingConfiguration>());
    }

    /// <summary>
    /// Maps the Skoruba Duende IdentityServer Admin UI to the routes of this application.
    /// </summary>
    public static IEndpointConventionBuilder MapIdentityServerAdminUI(this IEndpointRouteBuilder endpoint, string patternPrefix = "/")
    {
        return endpoint.MapAreaControllerRoute(CommonConsts.AdminUIArea, CommonConsts.AdminUIArea, patternPrefix + "{controller=Home}/{action=Index}/{id?}");
    }

    /// <summary>
    /// Maps the Skoruba Duende IdentityServer Admin UI health checks to the routes of this application.
    /// </summary>
    public static IEndpointConventionBuilder MapIdentityServerAdminUIHealthChecks(this IEndpointRouteBuilder endpoint, string pattern = "/health", Action<HealthCheckOptions> configureAction = null)
    {
        HealthCheckOptions options = new() { ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse };

        configureAction?.Invoke(options);

        return endpoint.MapHealthChecks(pattern, options);
    }
}

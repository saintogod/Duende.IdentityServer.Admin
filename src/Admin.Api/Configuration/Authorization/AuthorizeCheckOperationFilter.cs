// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace Skoruba.Duende.IdentityServer.Admin.Api.Configuration;

internal sealed class AuthorizeCheckOperationFilter : IOperationFilter
{
    private readonly AdminApiConfiguration adminApiConfiguration;

    public AuthorizeCheckOperationFilter(IOptions<AdminApiConfiguration> adminApiConfiguration)
    {
        this.adminApiConfiguration = adminApiConfiguration.Value;
    }

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var hasAuthorize = context.MethodInfo.DeclaringType is not null
            && (context.MethodInfo.DeclaringType.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any() // the controller has [Authorize]
                || context.MethodInfo.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any());// the method has [Authorize]

        if (hasAuthorize)
        {
            operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
            operation.Responses.Add("403", new OpenApiResponse { Description = "Forbidden" });

            operation.Security = new List<OpenApiSecurityRequirement>
            {
                new ()
                {
                    {
                        new () { Reference = new () { Type = ReferenceType.SecurityScheme, Id = "oauth2" } }, 
                        new[] { adminApiConfiguration.OidcApiName }
                    }
                }
            };
        }
    }
}
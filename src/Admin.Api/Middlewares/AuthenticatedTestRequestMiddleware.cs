// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using IdentityModel;

using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Skoruba.Duende.IdentityServer.Admin.Api.Middlewares;

public class AuthenticatedTestRequestMiddleware
{
    private readonly RequestDelegate next;
    public static readonly string TestAuthorizationHeader = "FakeAuthorization";
    public AuthenticatedTestRequestMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue(TestAuthorizationHeader, out var tokens))
        {
            var token = tokens.Single();
            var jwt = new JwtSecurityToken(token);
            var claimsIdentity = new ClaimsIdentity(jwt.Claims, JwtBearerDefaults.AuthenticationScheme, JwtClaimTypes.Name, JwtClaimTypes.Role);
            context.User = new ClaimsPrincipal(claimsIdentity);
        }

        await next(context);
    }
}
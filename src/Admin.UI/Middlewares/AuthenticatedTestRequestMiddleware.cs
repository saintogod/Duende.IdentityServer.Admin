﻿// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using Microsoft.AspNetCore.Http;

namespace Skoruba.Duende.IdentityServer.Admin.UI.Middlewares;

public class AuthenticatedTestRequestMiddleware
{
    private readonly RequestDelegate _next;
    public static readonly string TestAuthorizationHeader = "FakeAuthorization";
    public AuthenticatedTestRequestMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue(TestAuthorizationHeader, out var header))
        {
            var token = header.Single();
            var jwt = new JwtSecurityToken(token);
            var claimsIdentity = new ClaimsIdentity(jwt.Claims, "Cookies");
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            context.User = claimsPrincipal;
        }

        await _next(context);
    }
}
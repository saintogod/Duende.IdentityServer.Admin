﻿// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using Microsoft.AspNetCore.Http.Extensions;

using Skoruba.AuditLogging.Events;

namespace Skoruba.Duende.IdentityServer.Admin.Api.Configuration;

public class ApiAuditAction : IAuditAction
{
    public ApiAuditAction(IHttpContextAccessor accessor)
    {
        Action = new
        {
            accessor.HttpContext.TraceIdentifier,
            RequestUrl = accessor.HttpContext.Request.GetDisplayUrl(),
            HttpMethod = accessor.HttpContext.Request.Method
        };
    }

    public object Action { get; set; }
}
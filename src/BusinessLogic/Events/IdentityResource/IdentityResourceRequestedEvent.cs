﻿// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using Skoruba.AuditLogging.Events;
using Skoruba.Duende.IdentityServer.Admin.BusinessLogic.Dtos.Configuration;

namespace Skoruba.Duende.IdentityServer.Admin.BusinessLogic.Events.IdentityResource;

public class IdentityResourceRequestedEvent : AuditEvent
{
    public IdentityResourceDto IdentityResource { get; set; }

    public IdentityResourceRequestedEvent(IdentityResourceDto identityResource)
    {
        IdentityResource = identityResource;
    }
}
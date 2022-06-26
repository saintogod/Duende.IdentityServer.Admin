﻿// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using Skoruba.AuditLogging.Events;
using Skoruba.Duende.IdentityServer.Admin.BusinessLogic.Dtos.Configuration;

namespace Skoruba.Duende.IdentityServer.Admin.BusinessLogic.Events.IdentityResource;

public class IdentityResourcePropertyAddedEvent : AuditEvent
{
    public IdentityResourcePropertiesDto IdentityResourceProperty { get; set; }

    public IdentityResourcePropertyAddedEvent(IdentityResourcePropertiesDto identityResourceProperty)
    {
        IdentityResourceProperty = identityResourceProperty;
    }
}
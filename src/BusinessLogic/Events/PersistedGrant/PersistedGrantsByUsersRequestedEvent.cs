﻿// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using Skoruba.AuditLogging.Events;
using Skoruba.Duende.IdentityServer.Admin.BusinessLogic.Dtos.Grant;

namespace Skoruba.Duende.IdentityServer.Admin.BusinessLogic.Events.PersistedGrant;

public class PersistedGrantsByUsersRequestedEvent : AuditEvent
{
    public PersistedGrantsDto PersistedGrants { get; set; }

    public PersistedGrantsByUsersRequestedEvent(PersistedGrantsDto persistedGrants)
    {
        PersistedGrants = persistedGrants;
    }
}
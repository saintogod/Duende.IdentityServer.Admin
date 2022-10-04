// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

namespace Skoruba.Duende.IdentityServer.Shared.Configuration.Configuration.Identity;

public sealed record RegisterConfiguration(bool Enabled)
{
    public RegisterConfiguration() : this(true)
    {
    }
}

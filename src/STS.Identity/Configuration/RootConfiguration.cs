// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using Skoruba.Duende.IdentityServer.Shared.Configuration.Configuration.Identity;
using Skoruba.Duende.IdentityServer.STS.Identity.Configuration.Interfaces;

namespace Skoruba.Duende.IdentityServer.STS.Identity.Configuration;

public sealed record RootConfiguration : IRootConfiguration
{
    public AdminConfiguration AdminConfiguration { get; } = new ();

    public RegisterConfiguration RegisterConfiguration { get; } = new ();
}
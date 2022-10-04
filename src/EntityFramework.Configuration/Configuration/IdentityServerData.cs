// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using Duende.IdentityServer.Models;

using Client = Skoruba.Duende.IdentityServer.Admin.EntityFramework.Configuration.Configuration.IdentityServer.Client;

namespace Skoruba.Duende.IdentityServer.Admin.EntityFramework.Configuration.Configuration;

/// <summary>
/// Seed data for identity related data.
/// </summary>
public sealed record IdentityServerData
{
    public Client[] Clients { get; init; } = Array.Empty<Client>();
    public IdentityResource[] IdentityResources { get; init; } = Array.Empty<IdentityResource>();
    public ApiResource[] ApiResources { get; init; } = Array.Empty<ApiResource>();
    public ApiScope[] ApiScopes { get; init; } = Array.Empty<ApiScope>();
}

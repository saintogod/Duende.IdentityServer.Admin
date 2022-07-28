// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using Duende.IdentityServer.Models;

using Client = Skoruba.Duende.IdentityServer.Admin.EntityFramework.Configuration.Configuration.IdentityServer.Client;

namespace Skoruba.Duende.IdentityServer.Admin.EntityFramework.Configuration.Configuration;

public sealed record IdentityServerData
{
    public Client[] Clients { get; set; } = Array.Empty<Client>();
    public IdentityResource[] IdentityResources { get; set; } = Array.Empty<IdentityResource>();
    public ApiResource[] ApiResources { get; set; } = Array.Empty<ApiResource>();
    public ApiScope[] ApiScopes { get; set; } = Array.Empty<ApiScope>();
}

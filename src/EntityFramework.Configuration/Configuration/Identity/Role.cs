// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

namespace Skoruba.Duende.IdentityServer.Admin.EntityFramework.Configuration.Configuration.Identity;

public sealed record Role
{
    public string Name { get; set; }
    public List<Claim> Claims { get; set; } = new List<Claim>();
}

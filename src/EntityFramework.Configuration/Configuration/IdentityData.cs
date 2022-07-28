// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using Skoruba.Duende.IdentityServer.Admin.EntityFramework.Configuration.Configuration.Identity;

namespace Skoruba.Duende.IdentityServer.Admin.EntityFramework.Configuration.Configuration;

public sealed record IdentityData
{
    public Role[] Roles { get; set; } = Array.Empty<Role>();
    public User[] Users { get; set; } = Array.Empty<User>();
}

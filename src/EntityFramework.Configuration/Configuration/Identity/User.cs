﻿// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

namespace Skoruba.Duende.IdentityServer.Admin.EntityFramework.Configuration.Configuration.Identity;

public sealed record User
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public List<Claim> Claims { get; set; } = new List<Claim>();
    public List<string> Roles { get; set; } = new List<string>();
}

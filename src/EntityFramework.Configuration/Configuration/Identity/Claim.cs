// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

namespace Skoruba.Duende.IdentityServer.Admin.EntityFramework.Configuration.Configuration.Identity;

public sealed record Claim
{
    public string Type { get; set; }
    public string Value { get; set; }
}

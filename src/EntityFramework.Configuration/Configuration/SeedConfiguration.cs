﻿// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

namespace Skoruba.Duende.IdentityServer.Admin.EntityFramework.Configuration.Configuration;

public sealed record SeedConfiguration
{
    public bool ApplySeed { get; set; } = false;
}
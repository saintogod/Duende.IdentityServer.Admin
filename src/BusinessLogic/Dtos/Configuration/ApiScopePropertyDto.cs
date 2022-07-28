﻿// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

namespace Skoruba.Duende.IdentityServer.Admin.BusinessLogic.Dtos.Configuration;

public sealed record ApiScopePropertyDto
{
    public int Id { get; set; }
    public string Key { get; set; }
    public string Value { get; set; }
}
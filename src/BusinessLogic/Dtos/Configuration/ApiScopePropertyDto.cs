// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

namespace Skoruba.Duende.IdentityServer.Admin.BusinessLogic.Dtos.Configuration;

public sealed record ApiScopePropertyDto
{
    public int Id { get; init; }
    public string Key { get; init; }
    public string Value { get; init; }
}
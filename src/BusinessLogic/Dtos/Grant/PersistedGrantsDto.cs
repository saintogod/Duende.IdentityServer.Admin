// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

namespace Skoruba.Duende.IdentityServer.Admin.BusinessLogic.Dtos.Grant;

public sealed record PersistedGrantsDto
{
    public string SubjectId { get; init; }

    public int TotalCount { get; init; }

    public int PageSize { get; init; }

    public List<PersistedGrantDto> PersistedGrants { get; init; } = new();
}
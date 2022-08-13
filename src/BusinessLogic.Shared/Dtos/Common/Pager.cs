// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

namespace Skoruba.Duende.IdentityServer.Admin.BusinessLogic.Shared.Dtos.Common;

public sealed record Pager
{
    public int TotalCount { get; init; }

    public int PageSize { get; init; }

    public string Action { get; init; }

    public string Search { get; init; }

    public bool EnableSearch { get; init; } = false;

    public int MaxPages { get; init; } = 10;
}
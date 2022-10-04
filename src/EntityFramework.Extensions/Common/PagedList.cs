// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

namespace Skoruba.Duende.IdentityServer.Admin.EntityFramework.Extensions;

public sealed record PagedList<T> where T : class
{
    public List<T> Data { get; set; } = new ();

    public int TotalCount { get; set; }

    public int PageSize { get; set; }
}

// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

namespace Skoruba.Duende.IdentityServer.Admin.EntityFramework.Entities;

public sealed record PersistedGrantDataView
{
    public string SubjectId { get; set; }

    public string SubjectName { get; set; }
}

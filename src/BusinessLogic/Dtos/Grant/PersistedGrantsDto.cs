﻿// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

namespace Skoruba.Duende.IdentityServer.Admin.BusinessLogic.Dtos.Grant;

public class PersistedGrantsDto
{
    public PersistedGrantsDto()
    {
        PersistedGrants = new List<PersistedGrantDto>();
    }

    public string SubjectId { get; set; }

    public int TotalCount { get; set; }

    public int PageSize { get; set; }

    public List<PersistedGrantDto> PersistedGrants { get; set; }
}
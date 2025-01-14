﻿// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using Skoruba.Duende.IdentityServer.Admin.EntityFramework.Entities;
using Skoruba.Duende.IdentityServer.Admin.EntityFramework.Extensions;

namespace Skoruba.Duende.IdentityServer.Admin.EntityFramework.Repositories;

public interface ILogRepository
{
    Task<PagedList<Log>> GetLogsAsync(string search, int page = 1, int pageSize = 10);

    Task DeleteLogsOlderThanAsync(DateTime deleteOlderThan);

    bool AutoSaveChanges { get; set; }
}
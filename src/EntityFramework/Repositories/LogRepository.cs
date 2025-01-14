﻿// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System.Linq.Expressions;

using Microsoft.EntityFrameworkCore;

using Skoruba.Duende.IdentityServer.Admin.EntityFramework.Entities;
using Skoruba.Duende.IdentityServer.Admin.EntityFramework.Extensions;

namespace Skoruba.Duende.IdentityServer.Admin.EntityFramework.Repositories;

internal class LogRepository<TDbContext> : ILogRepository
    where TDbContext : DbContext, IAdminLogDbContext
{
    protected readonly TDbContext DbContext;

    public LogRepository(TDbContext dbContext)
    {
        DbContext = dbContext;
    }

    public virtual async Task DeleteLogsOlderThanAsync(DateTime deleteOlderThan)
    {
        var logsToDelete = await DbContext.Logs.Where(x => x.TimeStamp < deleteOlderThan.Date).ToListAsync();

        if (logsToDelete.Count == 0) return;

        DbContext.Logs.RemoveRange(logsToDelete);

        await AutoSaveChangesAsync();
    }

    public virtual async Task<PagedList<Log>> GetLogsAsync(string search, int page = 1, int pageSize = 10)
    {
        Expression<Func<Log, bool>> searchCondition = x => x.LogEvent.Contains(search) || x.Message.Contains(search) || x.Exception.Contains(search);
        var logs = await DbContext.Logs
            .WhereIf(!string.IsNullOrEmpty(search), searchCondition)
            .PageBy(x => x.Id, page, pageSize)
            .ToListAsync();

        var pagedList = new PagedList<Log>
        {
            Data = logs,
            PageSize = pageSize,
            TotalCount = await DbContext.Logs.WhereIf(!string.IsNullOrEmpty(search), searchCondition).CountAsync()
        };

        return pagedList;
    }

    protected virtual async Task<int> AutoSaveChangesAsync()
    {
        return AutoSaveChanges ? await DbContext.SaveChangesAsync() : (int)SavedStatus.WillBeSavedExplicitly;
    }

    public virtual async Task<int> SaveAllChangesAsync()
    {
        return await DbContext.SaveChangesAsync();
    }

    public bool AutoSaveChanges { get; set; } = true;
}
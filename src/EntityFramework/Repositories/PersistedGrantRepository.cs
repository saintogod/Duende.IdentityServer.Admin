// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System.Linq.Expressions;

using Duende.IdentityServer.EntityFramework.Entities;

using Microsoft.EntityFrameworkCore;

using Skoruba.Duende.IdentityServer.Admin.EntityFramework.Entities;
using Skoruba.Duende.IdentityServer.Admin.EntityFramework.Extensions;

namespace Skoruba.Duende.IdentityServer.Admin.EntityFramework.Repositories;

internal class PersistedGrantRepository<TDbContext> : IPersistedGrantRepository
    where TDbContext : DbContext, IAdminPersistedGrantDbContext
{
    protected readonly TDbContext DbContext;

    public bool AutoSaveChanges { get; set; } = true;

    public PersistedGrantRepository(TDbContext dbContext)
    {
        DbContext = dbContext;
    }

    public virtual async Task<PagedList<PersistedGrantDataView>> GetPersistedGrantsByUsersAsync(string search, int page = 1, int pageSize = 10)
    {

        var persistedGrantByUsers = DbContext.PersistedGrants
            .Select(pe => new PersistedGrantDataView { SubjectId = pe.SubjectId, SubjectName = string.Empty })
            .Distinct();

        Expression<Func< PersistedGrantDataView, bool>> searchCondition = x => x.SubjectId.Contains(search);

        var persistedGrantsData = await persistedGrantByUsers.WhereIf(!string.IsNullOrEmpty(search), searchCondition).PageBy(x => x.SubjectId, page, pageSize).ToListAsync();
        var persistedGrantsDataCount = await persistedGrantByUsers.WhereIf(!string.IsNullOrEmpty(search), searchCondition).CountAsync();

        return new ()
        {
            Data = (persistedGrantsData),
            TotalCount = persistedGrantsDataCount,
            PageSize = pageSize
        };
    }

    public virtual async Task<PagedList<PersistedGrant>> GetPersistedGrantsByUserAsync(string subjectId, int page = 1, int pageSize = 10)
    {

        var persistedGrantsData = await DbContext.PersistedGrants
            .Where(x => x.SubjectId == subjectId)
            .Select(x => new PersistedGrant
            {
                SubjectId = x.SubjectId,
                Type = x.Type,
                Key = x.Key,
                ClientId = x.ClientId,
                Data = x.Data,
                Expiration = x.Expiration,
                CreationTime = x.CreationTime
            })
            .PageBy(x => x.SubjectId, page, pageSize)
            .ToListAsync();

        var persistedGrantsCount = await DbContext.PersistedGrants.CountAsync(x => x.SubjectId == subjectId);

        return new()
        {
            Data = persistedGrantsData,
            TotalCount = persistedGrantsCount,
            PageSize = pageSize
        };
    }

    public virtual Task<PersistedGrant> GetPersistedGrantAsync(string key)
    {
        return DbContext.PersistedGrants.SingleOrDefaultAsync(x => x.Key == key);
    }

    public virtual async Task<int> DeletePersistedGrantAsync(string key)
    {
        var persistedGrant = await GetPersistedGrantAsync(key);

        DbContext.PersistedGrants.Remove(persistedGrant);

        return await AutoSaveChangesAsync();
    }

    public virtual Task<bool> ExistsPersistedGrantsAsync(string subjectId)
    {
        return DbContext.PersistedGrants.AnyAsync(x => x.SubjectId == subjectId);
    }

    public virtual async Task<int> DeletePersistedGrantsAsync(string userId)
    {
        var grants = await DbContext.PersistedGrants.Where(x => x.SubjectId == userId).ToListAsync();

        DbContext.RemoveRange(grants);

        return await AutoSaveChangesAsync();
    }

    protected virtual async Task<int> AutoSaveChangesAsync()
    {
        return AutoSaveChanges ? await DbContext.SaveChangesAsync() : (int)SavedStatus.WillBeSavedExplicitly;
    }

    public virtual async Task<int> SaveAllChangesAsync()
    {
        return await DbContext.SaveChangesAsync();
    }
}
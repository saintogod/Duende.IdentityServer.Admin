// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using Duende.IdentityServer.EntityFramework.Entities;

using Microsoft.EntityFrameworkCore;

using Skoruba.Duende.IdentityServer.Admin.EntityFramework.Extensions;

namespace Skoruba.Duende.IdentityServer.Admin.EntityFramework.Repositories;

internal class KeyRepository<TDbContext> : IKeyRepository
    where TDbContext : DbContext, IAdminPersistedGrantDbContext
{
    protected readonly TDbContext DbContext;
    public bool AutoSaveChanges { get; set; } = true;

    public KeyRepository(TDbContext dbContext)
    {
        DbContext = dbContext;
    }

    public virtual async Task<PagedList<Key>> GetKeysAsync(int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var clients = await DbContext.Keys.PageBy(x => x.Id, page, pageSize)
            .AsNoTracking()
            .ToListAsync(cancellationToken: cancellationToken);

        return new ()
        {
            Data = (clients),
            TotalCount = await DbContext.Keys.CountAsync(cancellationToken: cancellationToken),
            PageSize = pageSize
        };
    }

    public virtual async Task<Key> GetKeyAsync(string id, CancellationToken cancellationToken = default)
    {
        return await DbContext.Keys.Where(x => x.Id == id)
            .AsNoTracking()
            .SingleOrDefaultAsync(cancellationToken: cancellationToken);
    }

    public virtual async Task DeleteKeyAsync(string id, CancellationToken cancellationToken = default)
    {
        var keyToDelete = await DbContext.Keys.SingleOrDefaultAsync(x => x.Id == id, cancellationToken: cancellationToken);

        DbContext.Keys.Remove(keyToDelete);

        await AutoSaveChangesAsync(cancellationToken);
    }

    public virtual async Task<bool> ExistsKeyAsync(string id, CancellationToken cancellationToken = default)
    {
        return await DbContext.Keys.Where(x => x.Id == id).AnyAsync(cancellationToken: cancellationToken);
    }

    public virtual async Task<int> SaveAllChangesAsync(CancellationToken cancellationToken = default)
    {
        return await DbContext.SaveChangesAsync(cancellationToken);
    }

    protected virtual async Task<int> AutoSaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return AutoSaveChanges ? await DbContext.SaveChangesAsync(cancellationToken) : (int)SavedStatus.WillBeSavedExplicitly;
    }
}
﻿using System.Linq.Expressions;

using Duende.IdentityServer.EntityFramework.Entities;

using Microsoft.EntityFrameworkCore;

using Skoruba.Duende.IdentityServer.Admin.EntityFramework.Extensions;

namespace Skoruba.Duende.IdentityServer.Admin.EntityFramework.Repositories;

public class IdentityProviderRepository<TDbContext> : IIdentityProviderRepository
    where TDbContext : DbContext, IAdminConfigurationDbContext
{
    protected readonly TDbContext DbContext;

    public bool AutoSaveChanges { get; set; } = true;

    public IdentityProviderRepository(TDbContext dbContext)
    {
        DbContext = dbContext;
    }

    public virtual async Task<PagedList<IdentityProvider>> GetIdentityProvidersAsync(string search, int page = 1, int pageSize = 10)
    {
        Expression<Func<IdentityProvider, bool>> searchCondition = x => x.Scheme.Contains(search) || x.DisplayName.Contains(search);

        var identityProviders = await DbContext.IdentityProviders
            .WhereIf(!string.IsNullOrEmpty(search), searchCondition)
            .PageBy(x => x.Scheme, page, pageSize)
            .ToListAsync();

        var pagedList = new PagedList<IdentityProvider>
        {
            Data = identityProviders,
            TotalCount = await DbContext.IdentityProviders.WhereIf(!string.IsNullOrEmpty(search), searchCondition).CountAsync(),
            PageSize = pageSize
        };

        return pagedList;
    }

    public virtual Task<IdentityProvider> GetIdentityProviderAsync(int identityProviderId)
    {
        return DbContext.IdentityProviders
            .Where(x => x.Id == identityProviderId)
            .AsNoTracking()
            .SingleOrDefaultAsync();
    }

    /// <summary>
    /// Add new identityProvider
    /// </summary>
    /// <param name="identityProvider"></param>
    /// <returns>This method return new identityProvider id</returns>
    public virtual async Task<int> AddIdentityProviderAsync(IdentityProvider identityProvider)
    {
        DbContext.IdentityProviders.Add(identityProvider);

        await AutoSaveChangesAsync();

        return identityProvider.Id;
    }

    public virtual async Task<bool> CanInsertIdentityProviderAsync(IdentityProvider identityProvider)
    {
        if (identityProvider.Id == 0)
        {
            var existsWithSameName = await DbContext.IdentityProviders.SingleOrDefaultAsync(x => x.Scheme == identityProvider.Scheme);
            return existsWithSameName == null;
        }
        else
        {
            var existsWithSameName = await DbContext.IdentityProviders.SingleOrDefaultAsync(x => x.Scheme == identityProvider.Scheme && x.Id != identityProvider.Id);
            return existsWithSameName == null;
        }
    }

    public virtual async Task<int> DeleteIdentityProviderAsync(IdentityProvider identityProvider)
    {
        var identityProviderToDelete = await DbContext.IdentityProviders.SingleOrDefaultAsync(x => x.Id == identityProvider.Id);

        DbContext.IdentityProviders.Remove(identityProviderToDelete);
        return await AutoSaveChangesAsync();
    }

    public virtual async Task<int> UpdateIdentityProviderAsync(IdentityProvider identityProvider)
    {
        //Update with new data
        DbContext.IdentityProviders.Update(identityProvider);

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

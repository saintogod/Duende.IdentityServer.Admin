// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System.Linq.Expressions;

using Duende.IdentityServer.EntityFramework.Entities;

using Microsoft.EntityFrameworkCore;

using Skoruba.Duende.IdentityServer.Admin.EntityFramework.Extensions;

using ApiResource = Duende.IdentityServer.EntityFramework.Entities.ApiResource;

namespace Skoruba.Duende.IdentityServer.Admin.EntityFramework.Repositories;

internal class ApiResourceRepository<TDbContext> : IApiResourceRepository
    where TDbContext : DbContext, IAdminConfigurationDbContext
{
    protected readonly TDbContext DbContext;

    public bool AutoSaveChanges { get; set; } = true;

    public ApiResourceRepository(TDbContext dbContext)
    {
        DbContext = dbContext;
    }

    public virtual async Task<PagedList<ApiResource>> GetApiResourcesAsync(string search, int page = 1, int pageSize = 10)
    {
        Expression<Func<ApiResource, bool>> searchCondition = x => x.Name.Contains(search);

        var apiResources = await DbContext.ApiResources
            .WhereIf(!string.IsNullOrEmpty(search), searchCondition)
            .PageBy(x => x.Name, page, pageSize)
            .ToListAsync();

        return new ()
        {
            Data = apiResources,
            TotalCount = await DbContext.ApiResources.WhereIf(!string.IsNullOrEmpty(search), searchCondition).CountAsync(),
            PageSize = pageSize
        };
    }

    public virtual Task<ApiResource> GetApiResourceAsync(int apiResourceId)
    {
        return DbContext.ApiResources
            .Include(x => x.Scopes)
            .Where(x => x.Id == apiResourceId)
            .AsNoTracking()
            .SingleOrDefaultAsync();
    }

    public virtual async Task<PagedList<ApiResourceProperty>> GetApiResourcePropertiesAsync(int apiResourceId, int page = 1, int pageSize = 10)
    {
        var properties = await DbContext.ApiResourceProperties
            .Where(x => x.ApiResource.Id == apiResourceId)
            .PageBy(x => x.Id, page, pageSize)
            .ToListAsync();

        return new ()
        {
            Data = (properties),
            TotalCount = await DbContext.ApiResourceProperties.CountAsync(x => x.ApiResource.Id == apiResourceId),
            PageSize = pageSize
        };
    }

    public virtual Task<ApiResourceProperty> GetApiResourcePropertyAsync(int apiResourcePropertyId)
    {
        return DbContext.ApiResourceProperties
            .Include(x => x.ApiResource)
            .Where(x => x.Id == apiResourcePropertyId)
            .SingleOrDefaultAsync();
    }

    public virtual async Task<int> AddApiResourcePropertyAsync(int apiResourceId, ApiResourceProperty apiResourceProperty)
    {
        var apiResource = await DbContext.ApiResources.SingleOrDefaultAsync(x => x.Id == apiResourceId);

        apiResourceProperty.ApiResource = apiResource;
        await DbContext.ApiResourceProperties.AddAsync(apiResourceProperty);

        return await AutoSaveChangesAsync();
    }

    public virtual async Task<int> DeleteApiResourcePropertyAsync(ApiResourceProperty apiResourceProperty)
    {
        var propertyToDelete = await DbContext.ApiResourceProperties.SingleOrDefaultAsync(x => x.Id == apiResourceProperty.Id);

        DbContext.ApiResourceProperties.Remove(propertyToDelete);
        return await AutoSaveChangesAsync();
    }

    public virtual async Task<bool> CanInsertApiResourceAsync(ApiResource apiResource)
    {
        if (apiResource.Id == 0)
        {
            var existsWithSameName = await DbContext.ApiResources.SingleOrDefaultAsync(x => x.Name == apiResource.Name);
            return existsWithSameName == null;
        }
        else
        {
            var existsWithSameName = await DbContext.ApiResources.SingleOrDefaultAsync(x => x.Name == apiResource.Name && x.Id != apiResource.Id);
            return existsWithSameName == null;
        }
    }

    public virtual async Task<bool> CanInsertApiResourcePropertyAsync(ApiResourceProperty apiResourceProperty)
    {
        var existsWithSameName = await DbContext.ApiResourceProperties
            .SingleOrDefaultAsync(x => x.Key == apiResourceProperty.Key
                                    && x.ApiResource.Id == apiResourceProperty.ApiResourceId);
        return existsWithSameName == null;
    }

    /// <summary>
    /// Add new api resource
    /// </summary>
    /// <param name="apiResource"></param>
    /// <returns>This method return new api resource id</returns>
    public virtual async Task<int> AddApiResourceAsync(ApiResource apiResource)
    {
        DbContext.ApiResources.Add(apiResource);

        await AutoSaveChangesAsync();

        return apiResource.Id;
    }

    private async Task RemoveApiResourceClaimsAsync(ApiResource apiResource)
    {
        //Remove old api resource claims
        var apiResourceClaims = await DbContext.ApiResourceClaims.Where(x => x.ApiResource.Id == apiResource.Id).ToListAsync();
        DbContext.ApiResourceClaims.RemoveRange(apiResourceClaims);
    }

    private async Task RemoveApiResourceScopesAsync(ApiResource apiResource)
    {
        //Remove old api resource scopes
        var apiResourceScopes = await DbContext.ApiResourceScopes.Where(x => x.ApiResource.Id == apiResource.Id).ToListAsync();
        DbContext.ApiResourceScopes.RemoveRange(apiResourceScopes);
    }

    public virtual async Task<int> UpdateApiResourceAsync(ApiResource apiResource)
    {
        //Remove old relations
        await RemoveApiResourceClaimsAsync(apiResource);
        await RemoveApiResourceScopesAsync(apiResource);

        //Update with new data
        DbContext.ApiResources.Update(apiResource);

        return await AutoSaveChangesAsync();
    }

    public virtual async Task<int> DeleteApiResourceAsync(ApiResource apiResource)
    {
        var resource = await DbContext.ApiResources.SingleOrDefaultAsync();

        DbContext.Remove(resource);

        return await AutoSaveChangesAsync();
    }


    public virtual async Task<PagedList<ApiResourceSecret>> GetApiSecretsAsync(int apiResourceId, int page = 1, int pageSize = 10)
    {
        var apiSecrets = await DbContext.ApiSecrets.Where(x => x.ApiResource.Id == apiResourceId).PageBy(x => x.Id, page, pageSize).ToListAsync();

        return new ()
        {
            Data = apiSecrets,
            TotalCount = await DbContext.ApiSecrets.Where(x => x.ApiResource.Id == apiResourceId).CountAsync(),
            PageSize = pageSize
        };
    }

    public virtual Task<ApiResourceSecret> GetApiSecretAsync(int apiSecretId)
    {
        return DbContext.ApiSecrets
            .Include(x => x.ApiResource)
            .Where(x => x.Id == apiSecretId)
            .AsNoTracking()
            .SingleOrDefaultAsync();
    }

    public virtual async Task<int> AddApiSecretAsync(int apiResourceId, ApiResourceSecret apiSecret)
    {
        apiSecret.ApiResource = await DbContext.ApiResources.SingleOrDefaultAsync(x => x.Id == apiResourceId);
        await DbContext.ApiSecrets.AddAsync(apiSecret);

        return await AutoSaveChangesAsync();
    }

    public virtual async Task<int> DeleteApiSecretAsync(ApiResourceSecret apiSecret)
    {
        var apiSecretToDelete = await DbContext.ApiSecrets.SingleOrDefaultAsync(x => x.Id == apiSecret.Id);
        DbContext.ApiSecrets.Remove(apiSecretToDelete);

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

    public virtual async Task<string> GetApiResourceNameAsync(int apiResourceId)
    {
        var apiResource = await DbContext.ApiResources
            .SingleOrDefaultAsync(x => x.Id == apiResourceId);

        return apiResource.Name;
    }
}
﻿// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System.Linq.Expressions;

using Duende.IdentityServer.EntityFramework.Entities;
using Duende.IdentityServer.Models;

using Microsoft.EntityFrameworkCore;

using Skoruba.Duende.IdentityServer.Admin.EntityFramework.Constants;
using Skoruba.Duende.IdentityServer.Admin.EntityFramework.Extensions;
using Skoruba.Duende.IdentityServer.Admin.EntityFramework.Helpers;

using Client = Duende.IdentityServer.EntityFramework.Entities.Client;
using ClientClaim = Duende.IdentityServer.EntityFramework.Entities.ClientClaim;

namespace Skoruba.Duende.IdentityServer.Admin.EntityFramework.Repositories;

internal class ClientRepository<TDbContext> : IClientRepository
    where TDbContext : DbContext, IAdminConfigurationDbContext
{
    protected readonly TDbContext DbContext;
    public bool AutoSaveChanges { get; set; } = true;

    public ClientRepository(TDbContext dbContext)
    {
        DbContext = dbContext;
    }

    public virtual Task<Client> GetClientAsync(int clientId)
    {
        return DbContext.Clients
            .Include(x => x.AllowedGrantTypes)
            .Include(x => x.RedirectUris)
            .Include(x => x.PostLogoutRedirectUris)
            .Include(x => x.AllowedScopes)
            .Include(x => x.ClientSecrets)
            .Include(x => x.Claims)
            .Include(x => x.IdentityProviderRestrictions)
            .Include(x => x.AllowedCorsOrigins)
            .Include(x => x.Properties)
            .Where(x => x.Id == clientId)
            .AsNoTracking()
            .SingleOrDefaultAsync();
    }

    public virtual async Task<PagedList<Client>> GetClientsAsync(string search = "", int page = 1, int pageSize = 10)
    {
        var pagedList = new PagedList<Client>();

        Expression<Func<Client, bool>> searchCondition = x => x.ClientId.Contains(search) || x.ClientName.Contains(search);
        var clients = await DbContext.Clients.WhereIf(!string.IsNullOrEmpty(search), searchCondition).PageBy(x => x.Id, page, pageSize).ToListAsync();
        pagedList.Data = (clients);
        pagedList.TotalCount = await DbContext.Clients.WhereIf(!string.IsNullOrEmpty(search), searchCondition).CountAsync();
        pagedList.PageSize = pageSize;

        return pagedList;
    }

    public virtual async Task<List<string>> GetScopesAsync(string scope, int limit = 0)
    {
        var identityResources = await DbContext.IdentityResources
            .WhereIf(!string.IsNullOrEmpty(scope), x => x.Name.Contains(scope))
            .TakeIf(x => x.Id, limit > 0, limit)
            .Select(x => x.Name).ToListAsync();

        var apiScopes = await DbContext.ApiScopes
            .WhereIf(!string.IsNullOrEmpty(scope), x => x.Name.Contains(scope))
            .TakeIf(x => x.Id, limit > 0, limit)
            .Select(x => x.Name).ToListAsync();

        var scopes = identityResources.Concat(apiScopes).TakeIf(x => x, limit > 0, limit).ToList();

        return scopes;
    }

    public virtual List<string> GetGrantTypes(string grant, int limit = 0)
    {
        var filteredGrants = ClientConsts.GetGrantTypes()
            .WhereIf(!string.IsNullOrWhiteSpace(grant), x => x.Contains(grant))
            .TakeIf(x => x, limit > 0, limit)
            .ToList();

        return filteredGrants;
    }

    public virtual List<string> GetSigningAlgorithms(string algorithm, int limit = 0)
    {
        var signingAlgorithms = ClientConsts.SigningAlgorithms()
            .WhereIf(!string.IsNullOrWhiteSpace(algorithm), x => x.Contains(algorithm))
            .TakeIf(x => x, limit > 0, limit)
            .OrderBy(x => x)
            .ToList();

        return signingAlgorithms;
    }

    public virtual List<SelectItem> GetProtocolTypes()
    {
        return ClientConsts.GetProtocolTypes().ToList();
    }

    public virtual List<SelectItem> GetSecretTypes()
    {
        var secrets = ClientConsts.GetSecretTypes().Select(x => new SelectItem(x, x)).ToList();

        return secrets;
    }

    public virtual List<string> GetStandardClaims(string claim, int limit = 0)
    {
        var filteredClaims = ClientConsts.GetStandardClaims()
            .WhereIf(!string.IsNullOrWhiteSpace(claim), x => x.Contains(claim))
            .TakeIf(x => x, limit > 0, limit)
            .ToList();

        return filteredClaims;
    }

    public virtual List<SelectItem> GetAccessTokenTypes()
    {
        var accessTokenTypes = EnumHelpers.ToSelectList<AccessTokenType>();
        return accessTokenTypes;
    }

    public virtual List<SelectItem> GetTokenExpirations()
    {
        var tokenExpirations = EnumHelpers.ToSelectList<TokenExpiration>();
        return tokenExpirations;
    }

    public virtual List<SelectItem> GetTokenUsage()
    {
        var tokenUsage = EnumHelpers.ToSelectList<TokenUsage>();
        return tokenUsage;
    }

    public virtual List<SelectItem> GetHashTypes()
    {
        var hashTypes = EnumHelpers.ToSelectList<HashType>();
        return hashTypes;
    }

    public virtual async Task<int> AddClientSecretAsync(int clientId, ClientSecret clientSecret)
    {
        var client = await DbContext.Clients.SingleOrDefaultAsync(x => x.Id == clientId);
        clientSecret.Client = client;

        await DbContext.ClientSecrets.AddAsync(clientSecret);

        return await AutoSaveChangesAsync();
    }

    protected virtual async Task<int> AutoSaveChangesAsync()
    {
        return AutoSaveChanges ? await DbContext.SaveChangesAsync() : (int)SavedStatus.WillBeSavedExplicitly;
    }

    public virtual Task<ClientProperty> GetClientPropertyAsync(int clientPropertyId)
    {
        return DbContext.ClientProperties
            .Include(x => x.Client)
            .SingleOrDefaultAsync(x => x.Id == clientPropertyId);
    }

    public virtual async Task<int> AddClientClaimAsync(int clientId, ClientClaim clientClaim)
    {
        var client = await DbContext.Clients.SingleOrDefaultAsync(x => x.Id == clientId);

        clientClaim.Client = client;
        await DbContext.ClientClaims.AddAsync(clientClaim);

        return await AutoSaveChangesAsync();
    }

    public virtual async Task<int> AddClientPropertyAsync(int clientId, ClientProperty clientProperty)
    {
        var client = await DbContext.Clients.SingleOrDefaultAsync(x => x.Id == clientId);

        clientProperty.Client = client;
        await DbContext.ClientProperties.AddAsync(clientProperty);

        return await AutoSaveChangesAsync();
    }

    public virtual async Task<(string ClientId, string ClientName)> GetClientIdAsync(int clientId)
    {
        var client = await DbContext.Clients.Where(x => x.Id == clientId)
            .Select(x => new { x.ClientId, x.ClientName })
            .SingleOrDefaultAsync();

        return (client?.ClientId, client?.ClientName);
    }

    public virtual async Task<PagedList<ClientSecret>> GetClientSecretsAsync(int clientId, int page = 1, int pageSize = 10)
    {
        var secrets = await DbContext.ClientSecrets
            .Where(x => x.Client.Id == clientId)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        return new PagedList<ClientSecret>
        {
            Data = (secrets),
            TotalCount = await DbContext.ClientSecrets.CountAsync(x => x.Client.Id == clientId),
            PageSize = pageSize
        };
    }

    public virtual Task<ClientSecret> GetClientSecretAsync(int clientSecretId)
    {
        return DbContext.ClientSecrets
            .Include(x => x.Client)
            .Where(x => x.Id == clientSecretId)
            .AsNoTracking()
            .SingleOrDefaultAsync();
    }

    public virtual async Task<PagedList<ClientClaim>> GetClientClaimsAsync(int clientId, int page = 1, int pageSize = 10)
    {

        var claims = await DbContext.ClientClaims.Where(x => x.Client.Id == clientId).PageBy(x => x.Id, page, pageSize)
            .ToListAsync();

        return new ()
        {
            Data = (claims),
            TotalCount = await DbContext.ClientClaims.Where(x => x.Client.Id == clientId).CountAsync(),
            PageSize = pageSize
        };
    }

    public virtual async Task<PagedList<ClientProperty>> GetClientPropertiesAsync(int clientId, int page = 1, int pageSize = 10)
    {
        var properties = await DbContext.ClientProperties.Where(x => x.Client.Id == clientId).PageBy(x => x.Id, page, pageSize)
            .ToListAsync();
        
        return new ()
        {
            Data = (properties),
            TotalCount = await DbContext.ClientProperties.Where(x => x.Client.Id == clientId).CountAsync(),
            PageSize = pageSize
        };
    }

    public virtual Task<ClientClaim> GetClientClaimAsync(int clientClaimId)
    {
        return DbContext.ClientClaims
            .Include(x => x.Client)
            .Where(x => x.Id == clientClaimId)
            .AsNoTracking()
            .SingleOrDefaultAsync();
    }

    public virtual async Task<int> DeleteClientSecretAsync(ClientSecret clientSecret)
    {
        var secretToDelete = await DbContext.ClientSecrets.SingleOrDefaultAsync(x => x.Id == clientSecret.Id);

        DbContext.ClientSecrets.Remove(secretToDelete);

        return await AutoSaveChangesAsync();
    }

    public virtual async Task<int> DeleteClientClaimAsync(ClientClaim clientClaim)
    {
        var claimToDelete = await DbContext.ClientClaims.SingleOrDefaultAsync(x => x.Id == clientClaim.Id);

        DbContext.ClientClaims.Remove(claimToDelete);
        return await AutoSaveChangesAsync();
    }

    public virtual async Task<int> DeleteClientPropertyAsync(ClientProperty clientProperty)
    {
        var propertyToDelete = await DbContext.ClientProperties.SingleOrDefaultAsync(x => x.Id == clientProperty.Id);

        DbContext.ClientProperties.Remove(propertyToDelete);
        return await AutoSaveChangesAsync();
    }

    public virtual async Task<int> SaveAllChangesAsync()
    {
        return await DbContext.SaveChangesAsync();
    }

    public virtual async Task<bool> CanInsertClientAsync(Client client, bool isCloned = false)
    {
        if (client.Id == 0 || isCloned)
        {
            var existsWithClientName = await DbContext.Clients.SingleOrDefaultAsync(x => x.ClientId == client.ClientId);
            return existsWithClientName == null;
        }
        else
        {
            var existsWithClientName = await DbContext.Clients.SingleOrDefaultAsync(x => x.ClientId == client.ClientId && x.Id != client.Id);
            return existsWithClientName == null;
        }
    }

    /// <summary>
    /// Add new client, this method doesn't save client secrets, client claims, client properties
    /// </summary>
    /// <param name="client"></param>
    /// <returns>This method return new client id</returns>
    public virtual async Task<int> AddClientAsync(Client client)
    {
        DbContext.Clients.Add(client);

        await AutoSaveChangesAsync();

        return client.Id;
    }

    public virtual async Task<int> CloneClientAsync(Client client,
        bool cloneClientCorsOrigins = true,
        bool cloneClientGrantTypes = true,
        bool cloneClientIdPRestrictions = true,
        bool cloneClientPostLogoutRedirectUris = true,
        bool cloneClientScopes = true,
        bool cloneClientRedirectUris = true,
        bool cloneClientClaims = true,
        bool cloneClientProperties = true
        )
    {
        var clientToClone = await DbContext.Clients
            .Include(x => x.AllowedGrantTypes)
            .Include(x => x.RedirectUris)
            .Include(x => x.PostLogoutRedirectUris)
            .Include(x => x.AllowedScopes)
            .Include(x => x.ClientSecrets)
            .Include(x => x.Claims)
            .Include(x => x.IdentityProviderRestrictions)
            .Include(x => x.AllowedCorsOrigins)
            .Include(x => x.Properties)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == client.Id);

        clientToClone.ClientName = client.ClientName;
        clientToClone.ClientId = client.ClientId;

        //Clean original ids
        clientToClone.Id = 0;
        clientToClone.AllowedCorsOrigins.ForEach(x => x.Id = 0);
        clientToClone.RedirectUris.ForEach(x => x.Id = 0);
        clientToClone.PostLogoutRedirectUris.ForEach(x => x.Id = 0);
        clientToClone.AllowedScopes.ForEach(x => x.Id = 0);
        clientToClone.ClientSecrets.ForEach(x => x.Id = 0);
        clientToClone.IdentityProviderRestrictions.ForEach(x => x.Id = 0);
        clientToClone.Claims.ForEach(x => x.Id = 0);
        clientToClone.AllowedGrantTypes.ForEach(x => x.Id = 0);
        clientToClone.Properties.ForEach(x => x.Id = 0);

        //Client secret will be skipped
        clientToClone.ClientSecrets.Clear();

        if (!cloneClientCorsOrigins)
        {
            clientToClone.AllowedCorsOrigins.Clear();
        }

        if (!cloneClientGrantTypes)
        {
            clientToClone.AllowedGrantTypes.Clear();
        }

        if (!cloneClientIdPRestrictions)
        {
            clientToClone.IdentityProviderRestrictions.Clear();
        }

        if (!cloneClientPostLogoutRedirectUris)
        {
            clientToClone.PostLogoutRedirectUris.Clear();
        }

        if (!cloneClientScopes)
        {
            clientToClone.AllowedScopes.Clear();
        }

        if (!cloneClientRedirectUris)
        {
            clientToClone.RedirectUris.Clear();
        }

        if (!cloneClientClaims)
        {
            clientToClone.Claims.Clear();
        }

        if (!cloneClientProperties)
        {
            clientToClone.Properties.Clear();
        }

        await DbContext.Clients.AddAsync(clientToClone);

        await AutoSaveChangesAsync();

        return clientToClone.Id;
    }

    private async Task RemoveClientRelationsAsync(Client client, bool updateClientClaims,
        bool updateClientProperties)
    {
        //Remove old allowed scopes
        var clientScopes = await DbContext.ClientScopes.Where(x => x.Client.Id == client.Id).ToListAsync();
        DbContext.ClientScopes.RemoveRange(clientScopes);

        //Remove old grant types
        var clientGrantTypes = await DbContext.ClientGrantTypes.Where(x => x.Client.Id == client.Id).ToListAsync();
        DbContext.ClientGrantTypes.RemoveRange(clientGrantTypes);

        //Remove old redirect uri
        var clientRedirectUris = await DbContext.ClientRedirectUris.Where(x => x.Client.Id == client.Id).ToListAsync();
        DbContext.ClientRedirectUris.RemoveRange(clientRedirectUris);

        //Remove old client cors
        var clientCorsOrigins = await DbContext.ClientCorsOrigins.Where(x => x.Client.Id == client.Id).ToListAsync();
        DbContext.ClientCorsOrigins.RemoveRange(clientCorsOrigins);

        //Remove old client id restrictions
        var clientIdPRestrictions = await DbContext.ClientIdPRestrictions.Where(x => x.Client.Id == client.Id).ToListAsync();
        DbContext.ClientIdPRestrictions.RemoveRange(clientIdPRestrictions);

        //Remove old client post logout redirect
        var clientPostLogoutRedirectUris = await DbContext.ClientPostLogoutRedirectUris.Where(x => x.Client.Id == client.Id).ToListAsync();
        DbContext.ClientPostLogoutRedirectUris.RemoveRange(clientPostLogoutRedirectUris);

        //Remove old client claims
        if (updateClientClaims)
        {
            var clientClaims = await DbContext.ClientClaims.Where(x => x.Client.Id == client.Id).ToListAsync();
            DbContext.ClientClaims.RemoveRange(clientClaims);
        }

        //Remove old client properties
        if (updateClientProperties)
        {
            var clientProperties = await DbContext.ClientProperties.Where(x => x.Client.Id == client.Id).ToListAsync();
            DbContext.ClientProperties.RemoveRange(clientProperties);
        }
    }

    public virtual async Task<int> UpdateClientAsync(Client client, bool updateClientClaims = false, bool updateClientProperties = false)
    {
        //Remove old relations
        await RemoveClientRelationsAsync(client, updateClientClaims, updateClientProperties);

        //Update with new data
        DbContext.Clients.Update(client);

        return await AutoSaveChangesAsync();
    }

    public virtual async Task<int> RemoveClientAsync(Client client)
    {
        DbContext.Clients.Remove(client);

        return await AutoSaveChangesAsync();
    }
}
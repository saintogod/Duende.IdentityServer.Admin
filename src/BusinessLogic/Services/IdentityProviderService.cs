using Skoruba.AuditLogging.Services;
using Skoruba.Duende.IdentityServer.Admin.BusinessLogic.Dtos.IdentityProvider;
using Skoruba.Duende.IdentityServer.Admin.BusinessLogic.Events.IdentityProvider;
using Skoruba.Duende.IdentityServer.Admin.BusinessLogic.Mappers;
using Skoruba.Duende.IdentityServer.Admin.BusinessLogic.Resources;
using Skoruba.Duende.IdentityServer.Admin.BusinessLogic.Services.Interfaces;
using Skoruba.Duende.IdentityServer.Admin.BusinessLogic.Shared.ExceptionHandling;
using Skoruba.Duende.IdentityServer.Admin.EntityFramework.Repositories.Interfaces;

namespace Skoruba.Duende.IdentityServer.Admin.BusinessLogic.Services;

public class IdentityProviderService : IIdentityProviderService
{
    protected readonly IIdentityProviderRepository identityProviderRepository;
    protected readonly IIdentityProviderServiceResources identityProviderServiceResources;
    protected readonly IAuditEventLogger auditEventLogger;

    public IdentityProviderService(IIdentityProviderRepository identityProviderRepository,
        IIdentityProviderServiceResources identityProviderServiceResources,
        IAuditEventLogger auditEventLogger)
    {
        this.identityProviderRepository = identityProviderRepository;
        this.identityProviderServiceResources = identityProviderServiceResources;
        this.auditEventLogger = auditEventLogger;
    }

    public virtual async Task<IdentityProvidersDto> GetIdentityProvidersAsync(string search, int page = 1, int pageSize = 10)
    {
        var pagedList = await identityProviderRepository.GetIdentityProvidersAsync(search, page, pageSize);
        var identityProviderDto = pagedList.ToModel();

        await auditEventLogger.LogEventAsync(new IdentityProvidersRequestedEvent(identityProviderDto));

        return identityProviderDto;
    }

    public virtual async Task<IdentityProviderDto> GetIdentityProviderAsync(int identityProviderId)
    {
        var identityProvider = await identityProviderRepository.GetIdentityProviderAsync(identityProviderId);
        if (identityProvider is null)
            throw new UserFriendlyErrorPageException(string.Format(identityProviderServiceResources.IdentityProviderDoesNotExist().Description, identityProviderId));

        var identityProviderDto = identityProvider.ToModel();

        await auditEventLogger.LogEventAsync(new IdentityProviderRequestedEvent(identityProviderDto));

        return identityProviderDto;
    }

    public virtual async Task<bool> CanInsertIdentityProviderAsync(IdentityProviderDto identityProvider)
    {
        var entity = identityProvider.ToEntity();

        return await identityProviderRepository.CanInsertIdentityProviderAsync(entity);
    }

    public virtual async Task<int> AddIdentityProviderAsync(IdentityProviderDto identityProvider)
    {
        if (!await CanInsertIdentityProviderAsync(identityProvider))
        {
            throw new UserFriendlyViewException(string.Format(identityProviderServiceResources.IdentityProviderExistsValue().Description, identityProvider.Scheme), identityProviderServiceResources.IdentityProviderExistsKey().Description, identityProvider);
        }

        var entity = identityProvider.ToEntity();

        var saved = await identityProviderRepository.AddIdentityProviderAsync(entity);

        await auditEventLogger.LogEventAsync(new IdentityProviderAddedEvent(identityProvider));

        return saved;
    }

    public virtual async Task<int> UpdateIdentityProviderAsync(IdentityProviderDto identityProvider)
    {
        if (!await CanInsertIdentityProviderAsync(identityProvider))
        {
            throw new UserFriendlyViewException(string.Format(identityProviderServiceResources.IdentityProviderExistsValue().Description, identityProvider.Scheme), identityProviderServiceResources.IdentityProviderExistsKey().Description, identityProvider);
        }

        var originalIdentityProvider = await GetIdentityProviderAsync(identityProvider.Id);

        //if (identityProvider.Properties == null)
        //{
        //    identityProvider.Properties = new List<IdentityProviderPropertyDto>(originalIdentityProvider.Properties);
        //}

        var entity = identityProvider.ToEntity();

        var updated = await identityProviderRepository.UpdateIdentityProviderAsync(entity);

        await auditEventLogger.LogEventAsync(new IdentityProviderUpdatedEvent(originalIdentityProvider, identityProvider));

        return updated;
    }

    public virtual async Task<int> DeleteIdentityProviderAsync(IdentityProviderDto identityProvider)
    {
        var entity = identityProvider.ToEntity();

        var deleted = await identityProviderRepository.DeleteIdentityProviderAsync(entity);

        await auditEventLogger.LogEventAsync(new IdentityProviderDeletedEvent(identityProvider));

        return deleted;
    }
}
using Skoruba.AuditLogging.Events;
using Skoruba.Duende.IdentityServer.Admin.BusinessLogic.Dtos.IdentityProvider;

namespace Skoruba.Duende.IdentityServer.Admin.BusinessLogic.Events.IdentityProvider;

public sealed class IdentityProvidersRequestedEvent : AuditEvent
{
    public IdentityProvidersDto IdentityProviders { get; init; }

    public IdentityProvidersRequestedEvent(IdentityProvidersDto identityProviders)
    {
        IdentityProviders = identityProviders;
    }
}
using Skoruba.AuditLogging.Events;
using Skoruba.Duende.IdentityServer.Admin.BusinessLogic.Dtos.IdentityProvider;

namespace Skoruba.Duende.IdentityServer.Admin.BusinessLogic.Events.IdentityProvider;

public sealed class IdentityProviderRequestedEvent : AuditEvent
{
    public IdentityProviderDto IdentityProvider { get; init; }

    public IdentityProviderRequestedEvent(IdentityProviderDto identityProvider)
    {
        IdentityProvider = identityProvider;
    }
}
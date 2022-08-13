using Skoruba.AuditLogging.Events;
using Skoruba.Duende.IdentityServer.Admin.BusinessLogic.Dtos.IdentityProvider;

namespace Skoruba.Duende.IdentityServer.Admin.BusinessLogic.Events.IdentityProvider;

public sealed class IdentityProviderAddedEvent : AuditEvent
{
    public IdentityProviderDto IdentityProvider { get; init; }

    public IdentityProviderAddedEvent(IdentityProviderDto identityProvider)
    {
        IdentityProvider = identityProvider;
    }
}

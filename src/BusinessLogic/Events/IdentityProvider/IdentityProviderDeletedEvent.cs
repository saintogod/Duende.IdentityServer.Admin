using Skoruba.AuditLogging.Events;
using Skoruba.Duende.IdentityServer.Admin.BusinessLogic.Dtos.IdentityProvider;

namespace Skoruba.Duende.IdentityServer.Admin.BusinessLogic.Events.IdentityProvider;

public sealed class IdentityProviderDeletedEvent : AuditEvent
{
    public IdentityProviderDto IdentityProvider { get; init; }

    public IdentityProviderDeletedEvent(IdentityProviderDto identityProvider)
    {
        IdentityProvider = identityProvider;
    }
}

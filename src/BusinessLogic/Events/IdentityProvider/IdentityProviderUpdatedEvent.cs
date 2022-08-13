using Skoruba.AuditLogging.Events;
using Skoruba.Duende.IdentityServer.Admin.BusinessLogic.Dtos.IdentityProvider;

namespace Skoruba.Duende.IdentityServer.Admin.BusinessLogic.Events.IdentityProvider;

public sealed class IdentityProviderUpdatedEvent : AuditEvent
{
    public IdentityProviderDto OriginalIdentityProvider { get; init; }
    public IdentityProviderDto IdentityProvider { get; init; }

    public IdentityProviderUpdatedEvent(IdentityProviderDto originalIdentityProvider, IdentityProviderDto identityProvider)
    {
        OriginalIdentityProvider = originalIdentityProvider;
        IdentityProvider = identityProvider;
    }
}

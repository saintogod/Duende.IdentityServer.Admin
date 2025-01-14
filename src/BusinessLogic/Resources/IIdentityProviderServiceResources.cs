﻿using Skoruba.Duende.IdentityServer.Admin.BusinessLogic.Helpers;

namespace Skoruba.Duende.IdentityServer.Admin.BusinessLogic.Resources;

public interface IIdentityProviderServiceResources
{
    ResourceMessage IdentityProviderDoesNotExist();

    ResourceMessage IdentityProviderExistsKey();

    ResourceMessage IdentityProviderExistsValue();

}

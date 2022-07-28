// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using Skoruba.Duende.IdentityServer.Shared.Configuration.Configuration.Identity;

namespace Skoruba.Duende.IdentityServer.STS.Identity.Helpers.Localization;

public static class LoginPolicyResolutionLocalizer
{
    public static string GetUserNameLocalizationKey(LoginResolutionPolicy policy)
    {
        return policy switch
        {
            LoginResolutionPolicy.Email => nameof(LoginResolutionPolicy.Email),
            _ => nameof(LoginResolutionPolicy.Username),
        };
    }
}

// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using Microsoft.AspNetCore.Identity;

using Skoruba.Duende.IdentityServer.Shared.Configuration.Configuration.Identity;

namespace Skoruba.Duende.IdentityServer.STS.Identity.Helpers;

public sealed class UserResolver<TUser> where TUser : class
{
    private readonly UserManager<TUser> userManager;
    private readonly LoginResolutionPolicy policy;

    public UserResolver(UserManager<TUser> userManager, LoginConfiguration configuration)
    {
        this.userManager = userManager;
        policy = configuration.ResolutionPolicy;
    }

    public async Task<TUser> GetUserAsync(string login)
    {
        return policy switch
        {
            LoginResolutionPolicy.Username => await userManager.FindByNameAsync(login),
            LoginResolutionPolicy.Email => await userManager.FindByEmailAsync(login),
            _ => null,
        };
    }
}

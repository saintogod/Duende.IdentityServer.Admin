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

    public Task<TUser> GetUserAsync(string login)
        => policy switch
        {
            LoginResolutionPolicy.Username => userManager.FindByNameAsync(login),
            LoginResolutionPolicy.Email => userManager.FindByEmailAsync(login),
            _ => Task.FromResult<TUser>(null),
        };
}

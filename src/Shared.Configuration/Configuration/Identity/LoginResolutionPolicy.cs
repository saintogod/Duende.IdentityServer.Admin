// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

namespace Skoruba.Duende.IdentityServer.Shared.Configuration.Configuration.Identity;

// 
// 
/// <summary>
/// From where should the login be sourced.
/// <para>by default it's sourced from Username</para>
/// </summary>
public enum LoginResolutionPolicy
{
    Username = 0,
    Email = 1
}
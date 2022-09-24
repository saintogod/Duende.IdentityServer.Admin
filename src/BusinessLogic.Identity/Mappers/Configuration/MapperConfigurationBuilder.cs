﻿// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using Microsoft.AspNetCore.Identity;

using Skoruba.Duende.IdentityServer.Admin.BusinessLogic.Identity.Dtos.Identity;

namespace Skoruba.Duende.IdentityServer.Admin.BusinessLogic.Identity.Mappers.Configuration;

internal sealed class MapperConfigurationBuilder : IMapperConfigurationBuilder
{
    public HashSet<Type> ProfileTypes { get; } = new ();

    public IMapperConfigurationBuilder AddProfilesType(HashSet<Type> profileTypes)
    {
        if (profileTypes is null) return this;

        foreach (var profileType in profileTypes)
        {
            ProfileTypes.Add(profileType);
        }

        return this;
    }

    public IMapperConfigurationBuilder UseIdentityMappingProfile<TUserDto, TRoleDto, TUser, TRole, TKey,
        TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken,
        TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
        TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto,
        TUserClaimDto, TRoleClaimDto>()
        where TUserDto : UserDto<TKey>
        where TRoleDto : RoleDto<TKey>
        where TUser : IdentityUser<TKey>
        where TRole : IdentityRole<TKey>
        where TKey : IEquatable<TKey>
        where TUserClaim : IdentityUserClaim<TKey>
        where TUserRole : IdentityUserRole<TKey>
        where TUserLogin : IdentityUserLogin<TKey>
        where TRoleClaim : IdentityRoleClaim<TKey>
        where TUserToken : IdentityUserToken<TKey>
        where TUsersDto : UsersDto<TUserDto, TKey>
        where TRolesDto : RolesDto<TRoleDto, TKey>
        where TUserRolesDto : UserRolesDto<TRoleDto, TKey>
        where TUserClaimsDto : UserClaimsDto<TUserClaimDto, TKey>
        where TUserProviderDto : UserProviderDto<TKey>
        where TUserProvidersDto : UserProvidersDto<TUserProviderDto, TKey>
        where TUserChangePasswordDto : UserChangePasswordDto<TKey>
        where TRoleClaimsDto : RoleClaimsDto<TRoleClaimDto, TKey>
        where TUserClaimDto : UserClaimDto<TKey>
        where TRoleClaimDto : RoleClaimDto<TKey>
    {
        ProfileTypes.Add(typeof(IdentityMapperProfile<TUserDto, TRoleDto, TUser, TRole, TKey,
            TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken,
            TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
            TUserProviderDto, TUserProvidersDto, TRoleClaimsDto, TUserClaimDto, TRoleClaimDto>));

        return this;
    }
}
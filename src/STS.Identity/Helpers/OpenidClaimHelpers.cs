// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System.Security.Claims;
using System.Text.Json.Nodes;

using IdentityModel;

using Skoruba.Duende.IdentityServer.STS.Identity.Configuration;

namespace Skoruba.Duende.IdentityServer.STS.Identity.Helpers;

public static class OpenIdClaimHelpers
{
    public static Claim ExtractAddressClaim(OpenIdProfile profile)
    {
        var addressJson = JsonNode.Parse("{}");
        if (!string.IsNullOrWhiteSpace(profile.StreetAddress))
        {
            addressJson[AddressClaimConstants.StreetAddress] = profile.StreetAddress;
        }

        if (!string.IsNullOrWhiteSpace(profile.Locality))
        {
            addressJson[AddressClaimConstants.Locality] = profile.Locality;
        }

        if (!string.IsNullOrWhiteSpace(profile.Region))
        {
            addressJson[AddressClaimConstants.Region] = profile.Region;
        }

        if (!string.IsNullOrWhiteSpace(profile.PostalCode))
        {
            addressJson[AddressClaimConstants.PostalCode] = profile.PostalCode;
        }

        if (!string.IsNullOrWhiteSpace(profile.Country))
        {
            addressJson[AddressClaimConstants.Country] = profile.Country;
        }

        return new Claim(JwtClaimTypes.Address, addressJson.AsArray().Count > 0 ? addressJson.ToJsonString() : string.Empty);
    }

    /// <summary>
    /// Map claims to OpenId Profile
    /// </summary>
    /// <param name="claims"></param>
    /// <returns></returns>
    public static OpenIdProfile ExtractProfileInfo(IList<Claim> claims)
    {
        var profile = new OpenIdProfile
        {
            FullName = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Name)?.Value,
            Website = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.WebSite)?.Value,
            Profile = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Profile)?.Value
        };

        var address = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Address)?.Value;

        if (address == null) return profile;

        var addressJson = JsonNode.Parse(address);
        if (addressJson[AddressClaimConstants.StreetAddress] is not null)
        {
            profile.StreetAddress = addressJson[AddressClaimConstants.StreetAddress].ToString();
        }

        if (addressJson[AddressClaimConstants.Locality] is not null)
        {
            profile.Locality = addressJson[AddressClaimConstants.Locality].ToString();
        }

        if (addressJson[AddressClaimConstants.Region] is not null)
        {
            profile.Region = addressJson[AddressClaimConstants.Region].ToString();
        }

        if (addressJson[AddressClaimConstants.PostalCode] is not null)
        {
            profile.PostalCode = addressJson[AddressClaimConstants.PostalCode].ToString();
        }

        if (addressJson[AddressClaimConstants.Country] is not null)
        {
            profile.Country = addressJson[AddressClaimConstants.Country].ToString();
        }

        return profile;
    }

    /// <summary>
    /// Get claims to remove
    /// </summary>
    /// <param name="oldProfile"></param>
    /// <param name="newProfile"></param>
    /// <returns></returns>
    public static IEnumerable<Claim> ExtractClaimsToRemove(OpenIdProfile oldProfile, OpenIdProfile newProfile)
    {
        if (string.IsNullOrWhiteSpace(newProfile.FullName) && !string.IsNullOrWhiteSpace(oldProfile.FullName))
        {
            yield return new Claim(JwtClaimTypes.Name, oldProfile.FullName);
        }

        if (string.IsNullOrWhiteSpace(newProfile.Website) && !string.IsNullOrWhiteSpace(oldProfile.Website))
        {
            yield return new Claim(JwtClaimTypes.WebSite, oldProfile.Website);
        }

        if (string.IsNullOrWhiteSpace(newProfile.Profile) && !string.IsNullOrWhiteSpace(oldProfile.Profile))
        {
            yield return new Claim(JwtClaimTypes.Profile, oldProfile.Profile);
        }

        var oldAddressClaim = ExtractAddressClaim(oldProfile);
        var newAddressClaim = ExtractAddressClaim(newProfile);

        if (string.IsNullOrWhiteSpace(newAddressClaim.Value) && !string.IsNullOrWhiteSpace(oldAddressClaim.Value))
        {
            yield return oldAddressClaim;
        }
    }

    /// <summary>
    /// Get claims to add
    /// </summary>
    /// <param name="oldProfile"></param>
    /// <param name="newProfile"></param>
    /// <returns></returns>
    public static IEnumerable<Claim> ExtractClaimsToAdd(OpenIdProfile oldProfile, OpenIdProfile newProfile)
    {
        if (!string.IsNullOrWhiteSpace(newProfile.FullName) && string.IsNullOrWhiteSpace(oldProfile.FullName))
        {
            yield return new Claim(JwtClaimTypes.Name, newProfile.FullName);
        }

        if (!string.IsNullOrWhiteSpace(newProfile.Website) && string.IsNullOrWhiteSpace(oldProfile.Website))
        {
            yield return new Claim(JwtClaimTypes.WebSite, newProfile.Website);
        }

        if (!string.IsNullOrWhiteSpace(newProfile.Profile) && string.IsNullOrWhiteSpace(oldProfile.Profile))
        {
            yield return new Claim(JwtClaimTypes.Profile, newProfile.Profile);
        }

        var oldAddressClaim = ExtractAddressClaim(oldProfile);
        var newAddressClaim = ExtractAddressClaim(newProfile);

        if (!string.IsNullOrWhiteSpace(newAddressClaim.Value) && string.IsNullOrWhiteSpace(oldAddressClaim.Value))
        {
            yield return newAddressClaim;
        }
    }

    /// <summary>
    /// Get claims to replace
    /// </summary>
    /// <param name="oldClaims"></param>
    /// <param name="newProfile"></param>
    /// <returns></returns>
    public static IEnumerable<Tuple<Claim, Claim>> ExtractClaimsToReplace(IList<Claim> oldClaims, OpenIdProfile newProfile)
    {
        var oldProfile = ExtractProfileInfo(oldClaims);

        if (!string.IsNullOrWhiteSpace(newProfile.FullName) && !string.IsNullOrWhiteSpace(oldProfile.FullName))
        {
            if (newProfile.FullName != oldProfile.FullName)
            {
                var oldClaim = oldClaims.First(x => x.Type == JwtClaimTypes.Name);
                var newClaim = new Claim(JwtClaimTypes.Name, newProfile.FullName);
                yield return new Tuple<Claim, Claim>(oldClaim, newClaim);
            }
        }

        if (!string.IsNullOrWhiteSpace(newProfile.Website) && !string.IsNullOrWhiteSpace(oldProfile.Website))
        {
            if (newProfile.Website != oldProfile.Website)
            {
                var oldClaim = oldClaims.First(x => x.Type == JwtClaimTypes.WebSite);
                var newClaim = new Claim(JwtClaimTypes.WebSite, newProfile.Website);
                yield return new Tuple<Claim, Claim>(oldClaim, newClaim);
            }
        }

        if (!string.IsNullOrWhiteSpace(newProfile.Profile) && !string.IsNullOrWhiteSpace(oldProfile.Profile))
        {
            if (newProfile.Profile != oldProfile.Profile)
            {
                var oldClaim = oldClaims.First(x => x.Type == JwtClaimTypes.Profile);
                var newClaim = new Claim(JwtClaimTypes.Profile, newProfile.Profile);
                yield return new Tuple<Claim, Claim>(oldClaim, newClaim);
            }
        }

        var oldAddressClaim = ExtractAddressClaim(oldProfile);
        var newAddressClaim = ExtractAddressClaim(newProfile);

        if (!string.IsNullOrWhiteSpace(newAddressClaim.Value) && !string.IsNullOrWhiteSpace(oldAddressClaim.Value))
        {
            if (newAddressClaim.Value != oldAddressClaim.Value)
            {
                var oldClaim = oldClaims.First(x => x.Type == JwtClaimTypes.Address);
                yield return new Tuple<Claim, Claim>(oldClaim, newAddressClaim);
            }
        }
    }
}

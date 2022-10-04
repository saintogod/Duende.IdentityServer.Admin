// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using Skoruba.Duende.IdentityServer.Admin.EntityFramework.Extensions;

namespace Skoruba.Duende.IdentityServer.Admin.EntityFramework.Constants;

public static class ClientConsts
{
    public static IEnumerable<string> GetSecretTypes()
    {
        yield return "SharedSecret";
        yield return "X509Thumbprint";
        yield return "X509Name";
        yield return "X509CertificateBase64";
        yield return "JWK";
    }

    /// <summary>
    /// http://openid.net/specs/openid-connect-core-1_0.html#StandardClaims
    /// </summary>
    public static IEnumerable<string> GetStandardClaims()
    {
        yield return "name";
        yield return "given_name";
        yield return "family_name";
        yield return "middle_name";
        yield return "nickname";
        yield return "preferred_username";
        yield return "profile";
        yield return "picture";
        yield return "website";
        yield return "gender";
        yield return "birthdate";
        yield return "zoneinfo";
        yield return "locale";
        yield return "address";
        yield return "updated_at";
    }

    public static IEnumerable<string> GetGrantTypes()
    {
        yield return "implicit";
        yield return "client_credentials";
        yield return "authorization_code";
        yield return "hybrid";
        yield return "password";
        yield return "urn:ietf:params:oauth:grant-type:device_code";
        yield return "delegation";
    }

    public static IEnumerable<string> SigningAlgorithms()
    {
        yield return "RS256";
        yield return "RS384";
        yield return "RS512";
        yield return "PS256";
        yield return "PS384";
        yield return "PS512";
        yield return "ES256";
        yield return "ES384";
        yield return "ES512";
    }

    public static IEnumerable<SelectItem> GetProtocolTypes()
    {
        yield return new SelectItem("oidc", "OpenID Connect");
    }
}

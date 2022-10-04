// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System.Security.Cryptography;
using System.Text;

namespace Skoruba.Duende.IdentityServer.Shared.Helpers;

/// <summary>
/// Helper-class to create Md5hashes from strings
/// </summary>
public static class Md5HashHelper
{
    /// <summary>
    /// Computes a Md5-hash of the submitted string and returns the corresponding hash
    /// </summary>
    public static string GetHash(this string input)
    {
        using var md5 = MD5.Create();
        var bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(bytes);
    }
}

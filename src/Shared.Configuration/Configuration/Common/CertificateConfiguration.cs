// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

namespace Skoruba.Duende.IdentityServer.Shared.Configuration.Configuration.Common;

public sealed record CertificateConfiguration
{
    public bool UseTemporarySigningKeyForDevelopment { get; init; }

    public string CertificateStoreLocation { get; init; }
    public bool CertificateValidOnly { get; init; }

    public bool UseSigningCertificateThumbprint { get; init; }

    public string SigningCertificateThumbprint { get; init; }

    public bool UseSigningCertificatePfxFile { get; init; }

    public string SigningCertificatePfxFilePath { get; init; }

    public string SigningCertificatePfxFilePassword { get; init; }

    public bool UseValidationCertificateThumbprint { get; init; }

    public string ValidationCertificateThumbprint { get; init; }

    public bool UseValidationCertificatePfxFile { get; init; }

    public string ValidationCertificatePfxFilePath { get; init; }

    public string ValidationCertificatePfxFilePassword { get; init; }
}
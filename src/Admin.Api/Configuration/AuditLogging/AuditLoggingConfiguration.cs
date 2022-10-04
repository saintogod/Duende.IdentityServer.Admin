// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

namespace Skoruba.Duende.IdentityServer.Admin.Api.Configuration;

public sealed record AuditLoggingConfiguration
{
    public string Source { get; init; }

    public string SubjectIdentifierClaim { get; init; }

    public string SubjectNameClaim { get; init; }

    public string ClientIdClaim { get; init; }
}

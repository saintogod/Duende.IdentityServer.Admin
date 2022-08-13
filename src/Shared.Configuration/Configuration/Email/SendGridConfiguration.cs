// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

namespace Skoruba.Duende.IdentityServer.Shared.Configuration.Configuration.Email;

public sealed record SendGridConfiguration
{
    public string ApiKey { get; init; }
    public string SourceEmail { get; init; }
    public string SourceName { get; init; }
    public bool EnableClickTracking { get; init; } = false;
}
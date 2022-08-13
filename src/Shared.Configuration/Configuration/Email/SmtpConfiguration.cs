// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

namespace Skoruba.Duende.IdentityServer.Shared.Configuration.Configuration.Email;

public sealed record SmtpConfiguration
{
    public string From { get; init; }
    public string Host { get; init; }
    public string Login { get; init; }
    public string Password { get; init; }
    public int Port { get; init; } = 587; // default smtp port
    public bool UseSSL { get; init; } = true;
}
// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

namespace Skoruba.Duende.IdentityServer.Shared.Configuration.Configuration.Identity;

public sealed record LoginConfiguration(LoginResolutionPolicy ResolutionPolicy = LoginResolutionPolicy.Username);

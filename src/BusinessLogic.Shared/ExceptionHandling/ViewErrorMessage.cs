// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

namespace Skoruba.Duende.IdentityServer.Admin.BusinessLogic.Shared.ExceptionHandling;

public sealed record ViewErrorMessage
{
    public string ErrorKey { get; set; }

    public string ErrorMessage { get; set; }
}
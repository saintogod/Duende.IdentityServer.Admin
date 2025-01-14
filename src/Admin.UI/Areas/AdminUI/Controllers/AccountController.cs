﻿// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Skoruba.Duende.IdentityServer.Admin.UI.Configuration;

namespace Skoruba.Duende.IdentityServer.Admin.UI.Areas.AdminUI.Controllers;

[Authorize]
[Area(CommonConsts.AdminUIArea)]
public class AccountController : BaseController
{
    public AccountController(ILogger<ConfigurationController> logger) : base(logger)
    {
    }

    public IActionResult AccessDenied()
    {
        return View();
    }

    public static IActionResult Logout()
    {
        return new SignOutResult(new List<string> { AuthenticationConsts.SignInScheme, AuthenticationConsts.OidcAuthenticationScheme });
    }
}

﻿// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using Microsoft.AspNetCore.Mvc;

using Skoruba.Duende.IdentityServer.STS.Identity.Configuration;

namespace Skoruba.Duende.IdentityServer.STS.Identity.ViewComponents;

public class IdentityServerAdminLinkViewComponent : ViewComponent
{
    private readonly IRootConfiguration configuration;

    public IdentityServerAdminLinkViewComponent(IRootConfiguration configuration)
    {
        this.configuration = configuration;
    }

    public IViewComponentResult Invoke()
    {
        return View(model: configuration.AdminConfiguration.IdentityAdminBaseUrl);
    }
}
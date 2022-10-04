// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.

// Original file: https://github.com/DuendeSoftware/IdentityServer.Quickstart.UI
// Modified by Jan Škoruba

using Duende.IdentityServer.Models;

using Microsoft.AspNetCore.Mvc;

using Skoruba.Duende.IdentityServer.STS.Identity.ViewModels.Account;

namespace Skoruba.Duende.IdentityServer.STS.Identity.Helpers;

internal static class Extensions
{
    /// <summary>
    /// Checks if the redirect URI is for a native client.
    /// </summary>
    /// <returns></returns>
    public static bool IsNativeClient(this AuthorizationRequest context)
    {
        return !(context.RedirectUri.StartsWith(Uri.UriSchemeHttps, StringComparison.Ordinal)
               || context.RedirectUri.StartsWith(Uri.UriSchemeHttp, StringComparison.Ordinal));
    }

    public static IActionResult LoadingPage(this Controller controller, string viewName, string redirectUri)
    {
        controller.HttpContext.Response.StatusCode = 200;
        controller.HttpContext.Response.Headers.Location = string.Empty;

        return controller.View(viewName, new RedirectViewModel { RedirectUrl = redirectUri });
    }
}

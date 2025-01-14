﻿// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System.Text.RegularExpressions;

using Microsoft.Net.Http.Headers;

namespace Skoruba.Duende.IdentityServer.Admin.IntegrationTests.Common;

public class AntiforgeryHelper
{
    public static readonly string AntiForgeryFieldName = "__AFTField";
    public static readonly string AntiForgeryCookieName = "AFTCookie";

    public static async Task<(string fieldValue, string cookieValue)> ExtractAntiForgeryValues(HttpResponseMessage response)
    {
        return (ExtractAntiForgeryToken(await response.Content.ReadAsStringAsync()),
            ExtractAntiForgeryCookieValueFrom(response));
    }

    private static string ExtractAntiForgeryCookieValueFrom(HttpResponseMessage response)
    {
        var antiForgeryCookie = response.Headers.GetValues("Set-Cookie").FirstOrDefault(x => x.Contains(AntiForgeryCookieName));

        if (antiForgeryCookie is null)
        {
            throw new ArgumentException($"Cookie '{AntiForgeryCookieName}' not found in HTTP response", nameof(response));
        }

        var antiForgeryCookieValue = SetCookieHeaderValue.Parse(antiForgeryCookie).Value;

        return antiForgeryCookieValue.Value;
    }

    private static string ExtractAntiForgeryToken(string htmlBody)
    {
        var requestVerificationTokenMatch = Regex.Match(htmlBody, $@"\<input name=""{AntiForgeryFieldName}"" type=""hidden"" value=""([^""]+)"" \/\>");

        if (requestVerificationTokenMatch.Success)
        {
            return requestVerificationTokenMatch.Groups[1].Captures[0].Value;
        }

        throw new ArgumentException($"Anti forgery token '{AntiForgeryFieldName}' not found in HTML", nameof(htmlBody));
    }
}

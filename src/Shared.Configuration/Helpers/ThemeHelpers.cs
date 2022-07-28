// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

namespace Skoruba.Duende.IdentityServer.Shared.Configuration.Helpers;

public static class ThemeHelpers
{
    public const string CookieThemeKey = "AppTheme";

    public const string DefaultTheme = "default";

    private static readonly Lazy<string[]> themes = new(() => new[] { DefaultTheme, "darkly", "cosmo", "cerulean", "cyborg", "flatly", "journal", "litera", "lumen", "lux", "materia", "minty", "pulse", "sandstone", "simplex", "sketchy", "slate", "solar", "spacelab", "superhero", "united", "yeti" });

    public static ICollection<string> GetThemes()
    {
        return themes.Value;
    }
}
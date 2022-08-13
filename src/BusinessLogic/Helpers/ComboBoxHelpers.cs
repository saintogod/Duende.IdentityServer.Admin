// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System.Text.Json;

namespace Skoruba.Duende.IdentityServer.Admin.BusinessLogic.Helpers;

public static class ComboBoxHelpers
{
    public static void PopulateValuesToList(this List<string> list, string jsonValues)
    {
        if (string.IsNullOrEmpty(jsonValues)) return;

        var listValues = JsonSerializer.Deserialize<string[]>(jsonValues);
        if (listValues == null) return;

        list.AddRange(listValues);
    }
}
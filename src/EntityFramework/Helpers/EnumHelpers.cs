﻿// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using Skoruba.Duende.IdentityServer.Admin.EntityFramework.Extensions.Common;

namespace Skoruba.Duende.IdentityServer.Admin.EntityFramework.Helpers;

public static class EnumHelpers
{
    public static List<SelectItem> ToSelectList<T>() where T : struct, IComparable
    {
        var selectItems = Enum.GetValues(typeof(T))
            .Cast<T>()
            .Select(x => new SelectItem(Convert.ToInt16(x).ToString(), x.ToString())).ToList();

        return selectItems;
    }
}
// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

// https://github.com/aspnet/Extensions/blob/master/src/Localization/Abstractions/src/StringLocalizerOfT.cs
// Modified by Jan Škoruba

using System.Reflection;

using Microsoft.Extensions.Localization;

namespace Skoruba.Duende.IdentityServer.STS.Identity.Helpers.Localization;

public class GenericControllerLocalizer<TResourceSource> : IGenericControllerLocalizer<TResourceSource>
{
    private readonly IStringLocalizer localizer;

    /// <summary>
    /// Creates a new <see cref="T:Microsoft.Extensions.Localization.StringLocalizer`1" />.
    /// </summary>
    /// <param name="factory">The <see cref="T:Microsoft.Extensions.Localization.IStringLocalizerFactory" /> to use.</param>
    public GenericControllerLocalizer(IStringLocalizerFactory factory)
    {
        ArgumentNullException.ThrowIfNull(factory);

        var type = typeof(TResourceSource);
        var assemblyName = type.GetTypeInfo().Assembly.GetName().Name;
        var typeName = type.Name.Remove(type.Name.IndexOf('`'));
        var baseName = (type.Namespace + "." + typeName)[assemblyName.Length..].Trim('.');

        localizer = factory.Create(baseName, assemblyName);
    }

    public virtual LocalizedString this[string name]
    {
        get
        {
            ArgumentNullException.ThrowIfNull(name);
            return localizer[name];
        }
    }

    public virtual LocalizedString this[string name, params object[] arguments]
    {
        get
        {
            ArgumentNullException.ThrowIfNull(name);
            return localizer[name, arguments];
        }
    }

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
    {
        return localizer.GetAllStrings(includeParentCultures);
    }
}
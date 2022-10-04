// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System.Reflection;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace Skoruba.Duende.IdentityServer.STS.Identity.Configuration;

internal sealed class GenericTypeControllerFeatureProvider<TUser, TKey> : IApplicationFeatureProvider<ControllerFeature>
    where TUser : IdentityUser<TKey>
    where TKey : IEquatable<TKey>
{
    public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
    {
        var type = GetType();

        var controllerTypes = type.Assembly.GetExportedTypes()
            .Where(t => typeof(ControllerBase).IsAssignableFrom(t) && t.IsGenericTypeDefinition)
            .Select(t => t.GetTypeInfo());

        var genericType = type.GetGenericTypeDefinition().GetTypeInfo();
        var parameters = genericType.GenericTypeParameters
            .Select((p, i) => new { p.Name, Index = i })
            .ToDictionary(a => a.Name, a => type.GenericTypeArguments[a.Index]);

        foreach (var controllerType in controllerTypes)
        {
            var typeArguments = controllerType.GenericTypeParameters
                .Select(p => parameters[p.Name])
                .ToArray();

            feature.Controllers.Add(controllerType.MakeGenericType(typeArguments).GetTypeInfo());
        }
    }
}
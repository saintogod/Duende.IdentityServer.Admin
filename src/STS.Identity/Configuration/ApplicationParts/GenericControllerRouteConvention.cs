﻿// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Skoruba.Duende.IdentityServer.STS.Identity.Configuration;

public class GenericControllerRouteConvention : IControllerModelConvention
{
    public void Apply(ControllerModel controller)
    {
        if (controller.ControllerType.IsGenericType)
        {
            // this change is required because some of the controllers have generic parameters
            // and require resolution that will remove arity from the type 
            // as well as remove the 'Controller' at the end of string

            var name = controller.ControllerType.Name;
            var nameWithoutArity = name[..name.IndexOf('`')];
            controller.ControllerName = nameWithoutArity[..nameWithoutArity.LastIndexOf("Controller")];
        }
    }
}
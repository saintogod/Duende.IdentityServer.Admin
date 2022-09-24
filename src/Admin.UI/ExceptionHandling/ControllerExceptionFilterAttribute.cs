﻿// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System.Text.Json;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

using Skoruba.Duende.IdentityServer.Admin.BusinessLogic.Shared.ExceptionHandling;
using Skoruba.Duende.IdentityServer.Admin.UI.Helpers;

namespace Skoruba.Duende.IdentityServer.Admin.UI.ExceptionHandling;

public class ControllerExceptionFilterAttribute : ExceptionFilterAttribute
{
    private readonly ITempDataDictionaryFactory _tempDataDictionaryFactory;
    private readonly IModelMetadataProvider _modelMetadataProvider;

    public ControllerExceptionFilterAttribute(ITempDataDictionaryFactory tempDataDictionaryFactory,
        IModelMetadataProvider modelMetadataProvider)
    {
        _tempDataDictionaryFactory = tempDataDictionaryFactory;
        _modelMetadataProvider = modelMetadataProvider;
    }

    public override void OnException(ExceptionContext context)
    {
        if (context.Exception is not UserFriendlyErrorPageException &&
            context.Exception is not UserFriendlyViewException) return;

        //Create toastr notification
        if (CreateNotification(context, out var tempData)) return;

        HandleUserFriendlyViewException(context);
        ProcessException(context, tempData);

        //Clear toastr notification from temp
        ClearNotification(tempData);
    }

    private static void ClearNotification(ITempDataDictionary tempData)
    {
        tempData.Remove(NotificationHelpers.NotificationKey);
    }

    private bool CreateNotification(ExceptionContext context, out ITempDataDictionary tempData)
    {
        tempData = _tempDataDictionaryFactory.GetTempData(context.HttpContext);
        CreateNotification(NotificationHelpers.AlertType.Error, tempData, context.Exception.Message);

        return !tempData.ContainsKey(NotificationHelpers.NotificationKey);
    }

    private void ProcessException(ExceptionContext context, ITempDataDictionary tempData)
    {
        if (context.ActionDescriptor is not ControllerActionDescriptor controllerActionDescriptor) return;

        const string errorViewName = "Error";

        var result = new ViewResult
        {
            ViewName = context.Exception is UserFriendlyViewException
                ? controllerActionDescriptor.ActionName
                : errorViewName,
            TempData = tempData,
            ViewData = new ViewDataDictionary(_modelMetadataProvider, context.ModelState)
            {
                {"Notifications", tempData[NotificationHelpers.NotificationKey]},
            }
        };

        //For UserFriendlyException is necessary return model with latest form state
        if (context.Exception is UserFriendlyViewException exception)
        {
            result.ViewData.Model = exception.Model;
        }

        context.ExceptionHandled = true;
        context.Result = result;
    }

    private static void HandleUserFriendlyViewException(ExceptionContext context)
    {
        if (context.Exception is not UserFriendlyViewException userFriendlyViewException) return;

        if (userFriendlyViewException.ErrorMessages?.Any() == true)
        {
            foreach (var message in userFriendlyViewException.ErrorMessages)
            {
                context.ModelState.AddModelError(message.ErrorKey, message.ErrorMessage);
            }
            return;
        }
        context.ModelState.AddModelError(userFriendlyViewException.ErrorKey, context.Exception.Message);
    }

    protected static void CreateNotification(NotificationHelpers.AlertType type, ITempDataDictionary tempData, string message, string title = "")
    {
        var toast = new NotificationHelpers.Alert
        {
            Type = type,
            Message = message,
            Title = title
        };

        var alerts = new List<NotificationHelpers.Alert>();

        if (tempData.ContainsKey(NotificationHelpers.NotificationKey))
        {
            alerts = JsonSerializer.Deserialize<List<NotificationHelpers.Alert>>(tempData[NotificationHelpers.NotificationKey].ToString());
            tempData.Remove(NotificationHelpers.NotificationKey);
        }

        alerts.Add(toast);

        var alertJson = JsonSerializer.Serialize(alerts);

        tempData.Add(NotificationHelpers.NotificationKey, alertJson);
    }
}

// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System.ComponentModel.DataAnnotations;

namespace Skoruba.Duende.IdentityServer.Admin.BusinessLogic.Dtos.Log;

public class LogsDto
{
    [Required]
    public DateTime? DeleteOlderThan { get; set; }

    public LogDto[] Logs { get; set; } = Array.Empty<LogDto>();

    public int TotalCount { get; set; }

    public int PageSize { get; set; }
}
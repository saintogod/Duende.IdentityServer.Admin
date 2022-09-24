// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System.ComponentModel.DataAnnotations;

namespace Skoruba.Duende.IdentityServer.Admin.BusinessLogic.Dtos.Log;

public sealed record AuditLogsDto
{
    [Required]
    public DateTime? DeleteOlderThan { get; set; }

    public AuditLogDto[] Logs { get; set; } = Array.Empty<AuditLogDto>();

    public int TotalCount { get; set; }

    public int PageSize { get; set; }
}
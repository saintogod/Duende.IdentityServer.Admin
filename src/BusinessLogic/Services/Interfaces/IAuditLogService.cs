// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using Skoruba.Duende.IdentityServer.Admin.BusinessLogic.Dtos.Log;

namespace Skoruba.Duende.IdentityServer.Admin.BusinessLogic.Services;

public interface IAuditLogService
{
    Task<AuditLogsDto> GetAsync(AuditLogFilterDto filters);

    Task DeleteLogsOlderThanAsync(DateTime deleteOlderThan);
}
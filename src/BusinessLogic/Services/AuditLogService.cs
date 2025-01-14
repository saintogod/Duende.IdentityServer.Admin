﻿// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using Skoruba.AuditLogging.EntityFramework.Entities;
using Skoruba.Duende.IdentityServer.Admin.BusinessLogic.Dtos.Log;
using Skoruba.Duende.IdentityServer.Admin.BusinessLogic.Mappers;
using Skoruba.Duende.IdentityServer.Admin.EntityFramework.Repositories;

namespace Skoruba.Duende.IdentityServer.Admin.BusinessLogic.Services;

internal sealed class AuditLogService<TAuditLog> : IAuditLogService
    where TAuditLog : AuditLog
{
    private readonly IAuditLogRepository<TAuditLog> auditLogRepository;

    public AuditLogService(IAuditLogRepository<TAuditLog> auditLogRepository)
    {
        this.auditLogRepository = auditLogRepository;
    }

    public async Task<AuditLogsDto> GetAsync(AuditLogFilterDto filters)
    {
        var pagedList = await auditLogRepository.GetAsync(filters.Event, filters.Source, filters.Category, filters.Created, filters.SubjectIdentifier, filters.SubjectName, filters.Page, filters.PageSize);
        var auditLogsDto = pagedList.ToModel();

        return auditLogsDto;
    }

    public Task DeleteLogsOlderThanAsync(DateTime deleteOlderThan)
    {
        return auditLogRepository.DeleteLogsOlderThanAsync(deleteOlderThan);
    }
}
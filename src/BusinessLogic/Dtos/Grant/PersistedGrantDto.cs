// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

namespace Skoruba.Duende.IdentityServer.Admin.BusinessLogic.Dtos.Grant;

public sealed record PersistedGrantDto
{
    public string Key { get; init; }
    public string Type { get; init; }
    public string SubjectId { get; init; }
    public string SubjectName { get; init; }
    public string ClientId { get; init; }
    public DateTime CreationTime { get; init; }
    public DateTime? Expiration { get; init; }
    public string Data { get; init; }
    public DateTime? ConsumedTime { get; init; }
    public string SessionId { get; init; }
    public string Description { get; init; }
}
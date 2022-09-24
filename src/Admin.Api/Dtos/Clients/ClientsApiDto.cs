// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

namespace Skoruba.Duende.IdentityServer.Admin.Api.Dtos.Clients;

public class ClientsApiDto
{
    public ClientsApiDto()
    {
        Clients = new List<ClientApiDto>();
    }

    public List<ClientApiDto> Clients { get; set; }

    public int TotalCount { get; set; }

    public int PageSize { get; set; }
}
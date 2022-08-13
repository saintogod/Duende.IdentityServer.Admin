namespace Skoruba.Duende.IdentityServer.Admin.Api.Dtos.IdentityProvider;

public sealed record IdentityProvidersApiDto
{
    public int PageSize { get; init; }

    public int TotalCount { get; init; }

    public List<IdentityProviderApiDto> IdentityProviders { get; init; } = new List<IdentityProviderApiDto>();
}
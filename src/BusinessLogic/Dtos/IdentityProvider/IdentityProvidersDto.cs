namespace Skoruba.Duende.IdentityServer.Admin.BusinessLogic.Dtos.IdentityProvider;

public sealed record IdentityProvidersDto
{
    public int PageSize { get; init; }

    public int TotalCount { get; init; }

    public List<IdentityProviderDto> IdentityProviders { get; init; } = new();
}
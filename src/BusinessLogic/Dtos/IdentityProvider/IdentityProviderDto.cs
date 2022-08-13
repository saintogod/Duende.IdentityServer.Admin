using System.ComponentModel.DataAnnotations;

namespace Skoruba.Duende.IdentityServer.Admin.BusinessLogic.Dtos.IdentityProvider;

public sealed record IdentityProviderDto
{
    [Required]
    public string Type { get; init; } = "oidc";

    public int Id { get; init; }

    [Required]
    public string Scheme { get; init; }

    public string DisplayName { get; init; }

    public bool Enabled { get; init; } = true;

    public Dictionary<int, IdentityProviderPropertyDto> Properties { get; init; } = new();
}

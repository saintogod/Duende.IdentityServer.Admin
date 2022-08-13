using System.ComponentModel.DataAnnotations;

namespace Skoruba.Duende.IdentityServer.Admin.Api.Dtos.IdentityProvider;

public sealed record IdentityProviderApiDto
{
    public string Type { get; set; }

    public int Id { get; set; }

    [Required]
    public string Scheme { get; set; }

    public string DisplayName { get; set; }

    public bool Enabled { get; set; } = true;

    public Dictionary<string, string> IdentityProviderProperties { get; set; } = new();
}

using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace Skoruba.Duende.IdentityServer.Admin.BusinessLogic.Dtos.IdentityProvider;

[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
public sealed record IdentityProviderPropertyDto
{
    private string DebuggerDisplay => $"{Name} = {Value}";

    [Required]
    public string Name { get; set; }

    public string Value { get; set; } = "";
}
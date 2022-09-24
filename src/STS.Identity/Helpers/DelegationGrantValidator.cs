using Duende.IdentityServer.Models;
using Duende.IdentityServer.Validation;

using IdentityModel;

namespace Skoruba.Duende.IdentityServer.STS.Identity.Helpers;

internal sealed class DelegationGrantValidator : IExtensionGrantValidator
{
    private readonly ITokenValidator validator;

    public DelegationGrantValidator(ITokenValidator validator)
    {
        this.validator = validator;
    }

    public string GrantType => "delegation";

    public async Task ValidateAsync(ExtensionGrantValidationContext context)
    {
        var userToken = context.Request.Raw.Get("token");

        if (string.IsNullOrEmpty(userToken))
        {
            context.Result = new (TokenRequestErrors.InvalidGrant);
            return;
        }

        var result = await validator.ValidateAccessTokenAsync(userToken);
        if (result.IsError)
        {
            context.Result = new (TokenRequestErrors.InvalidGrant);
            return;
        }

        // get user's identity
        var sub = result.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.Subject).Value;

        context.Result = new (sub, GrantType);
        return;
    }
}
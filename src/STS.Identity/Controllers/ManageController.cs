using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

using Skoruba.Duende.IdentityServer.STS.Identity.Helpers;
using Skoruba.Duende.IdentityServer.STS.Identity.Helpers.Localization;
using Skoruba.Duende.IdentityServer.STS.Identity.ViewModels.Manage;

namespace Skoruba.Duende.IdentityServer.STS.Identity.Controllers;

[Authorize]
public class ManageController<TUser, TKey> : Controller
    where TUser : IdentityUser<TKey>, new()
    where TKey : IEquatable<TKey>
{
    private const string RecoveryCodesKey = nameof(RecoveryCodesKey);
    private const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";

    private readonly UserManager<TUser> userManager;
    private readonly SignInManager<TUser> signInManager;
    private readonly IEmailSender emailSender;
    private readonly ILogger<ManageController<TUser, TKey>> logger;
    private readonly IGenericControllerLocalizer<ManageController<TUser, TKey>> localizer;
    private readonly UrlEncoder urlEncoder;

    [TempData]
    public string StatusMessage { get; set; }

    public ManageController(UserManager<TUser> userManager, SignInManager<TUser> signInManager, IEmailSender emailSender, ILogger<ManageController<TUser, TKey>> logger, IGenericControllerLocalizer<ManageController<TUser, TKey>> localizer, UrlEncoder urlEncoder)
    {
        this.userManager = userManager;
        this.signInManager = signInManager;
        this.emailSender = emailSender;
        this.logger = logger;
        this.localizer = localizer;
        this.urlEncoder = urlEncoder;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var user = await userManager.GetUserAsync(User);

        if (user == null)
        {
            return NotFound(localizer["UserNotFound", userManager.GetUserId(User)]);
        }

        var model = await BuildManageIndexViewModelAsync(user);

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(IndexViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound(localizer["UserNotFound", userManager.GetUserId(User)]);
        }

        var email = user.Email;
        if (model.Email != email)
        {
            var setEmailResult = await userManager.SetEmailAsync(user, model.Email);
            if (!setEmailResult.Succeeded)
            {
                throw new ApplicationException(localizer["ErrorSettingEmail", user.Id]);
            }
        }

        var phoneNumber = user.PhoneNumber;
        if (model.PhoneNumber != phoneNumber)
        {
            var setPhoneResult = await userManager.SetPhoneNumberAsync(user, model.PhoneNumber);
            if (!setPhoneResult.Succeeded)
            {
                throw new ApplicationException(localizer["ErrorSettingPhone", user.Id]);
            }
        }

        await UpdateUserClaimsAsync(model, user);

        StatusMessage = localizer["ProfileUpdated"];

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SendVerificationEmail(IndexViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound(localizer["UserNotFound", userManager.GetUserId(User)]);
        }

        var code = await userManager.GenerateEmailConfirmationTokenAsync(user);

        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code }, HttpContext.Request.Scheme);

        await emailSender.SendEmailAsync(model.Email, localizer["ConfirmEmailTitle"], localizer["ConfirmEmailBody", HtmlEncoder.Default.Encode(callbackUrl)]);

        StatusMessage = localizer["VerificationSent"];

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> ChangePassword()
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound(localizer["UserNotFound", userManager.GetUserId(User)]);
        }

        var hasPassword = await userManager.HasPasswordAsync(user);
        if (!hasPassword)
        {
            return RedirectToAction(nameof(SetPassword));
        }

        var model = new ChangePasswordViewModel { StatusMessage = StatusMessage };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound(localizer["UserNotFound", userManager.GetUserId(User)]);
        }

        var changePasswordResult = await userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
        if (!changePasswordResult.Succeeded)
        {
            AddErrors(changePasswordResult);
            return View(model);
        }

        await signInManager.RefreshSignInAsync(user);
        logger.LogInformation(localizer["PasswordChangedLog", user.UserName]);

        StatusMessage = localizer["PasswordChanged"];

        return RedirectToAction(nameof(ChangePassword));
    }

    [HttpGet]
    public async Task<IActionResult> SetPassword()
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound(localizer["UserNotFound", userManager.GetUserId(User)]);
        }

        var hasPassword = await userManager.HasPasswordAsync(user);

        if (hasPassword)
        {
            return RedirectToAction(nameof(ChangePassword));
        }

        var model = new SetPasswordViewModel { StatusMessage = StatusMessage };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetPassword(SetPasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound(localizer["UserNotFound", userManager.GetUserId(User)]);
        }

        var addPasswordResult = await userManager.AddPasswordAsync(user, model.NewPassword);
        if (!addPasswordResult.Succeeded)
        {
            AddErrors(addPasswordResult);
            return View(model);
        }

        await signInManager.RefreshSignInAsync(user);
        StatusMessage = localizer["PasswordSet"];

        return RedirectToAction(nameof(SetPassword));
    }

    [HttpGet]
    public async Task<IActionResult> PersonalData()
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound(localizer["UserNotFound", userManager.GetUserId(User)]);
        }

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DownloadPersonalData()
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound(localizer["UserNotFound", userManager.GetUserId(User)]);
        }

        logger.LogInformation(localizer["AskForPersonalDataLog"], userManager.GetUserId(User));

        var personalDataProps = typeof(TUser).GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(PersonalDataAttribute)));
        var personalData = personalDataProps.ToDictionary(p => p.Name, p => p.GetValue(user)?.ToString() ?? "null");

        Response.Headers.Add("Content-Disposition", "attachment; filename=PersonalData.json");
        return new FileContentResult(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(personalData)), "text/json");
    }

    [HttpGet]
    public async Task<IActionResult> DeletePersonalData()
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound(localizer["UserNotFound", userManager.GetUserId(User)]);
        }

        var deletePersonalDataViewModel = new DeletePersonalDataViewModel
        {
            RequirePassword = await userManager.HasPasswordAsync(user)
        };

        return View(deletePersonalDataViewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeletePersonalData(DeletePersonalDataViewModel deletePersonalDataViewModel)
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound(localizer["UserNotFound", userManager.GetUserId(User)]);
        }

        deletePersonalDataViewModel.RequirePassword = await userManager.HasPasswordAsync(user);
        if (deletePersonalDataViewModel.RequirePassword)
        {
            if (!await userManager.CheckPasswordAsync(user, deletePersonalDataViewModel.Password))
            {
                ModelState.AddModelError(string.Empty, localizer["PasswordNotCorrect"]);
                return View(deletePersonalDataViewModel);
            }
        }

        var result = await userManager.DeleteAsync(user);
        var userId = await userManager.GetUserIdAsync(user);
        if (!result.Succeeded)
        {
            throw new InvalidOperationException(localizer["ErrorDeletingUser", user.Id]);
        }

        await signInManager.SignOutAsync();

        logger.LogInformation(localizer["DeletePersonalData"], userId);

        return Redirect("~/");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveLogin(RemoveLoginViewModel model)
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound(localizer["UserNotFound", userManager.GetUserId(User)]);
        }

        var result = await userManager.RemoveLoginAsync(user, model.LoginProvider, model.ProviderKey);
        if (!result.Succeeded)
        {
            throw new ApplicationException(localizer["ErrorRemovingExternalLogin", user.Id]);
        }

        await signInManager.RefreshSignInAsync(user);
        StatusMessage = localizer["ExternalLoginRemoved"];

        return RedirectToAction(nameof(ExternalLogins));
    }

    [HttpGet]
    public async Task<IActionResult> LinkLoginCallback()
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound(localizer["UserNotFound", userManager.GetUserId(User)]);
        }

        var info = await signInManager.GetExternalLoginInfoAsync(user.Id.ToString());
        if (info == null)
        {
            throw new ApplicationException(localizer["ErrorLoadingExternalLogin", user.Id]);
        }

        var result = await userManager.AddLoginAsync(user, info);
        if (!result.Succeeded)
        {
            AddErrors(result);
            return View("LinkLoginFailure");
        }

        // Clear the existing external cookie to ensure a clean login process
        await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
        StatusMessage = localizer["ExternalLoginAdded"];

        return RedirectToAction(nameof(ExternalLogins));
    }

    [HttpGet]
    public async Task<IActionResult> ExternalLogins()
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound(localizer["UserNotFound", userManager.GetUserId(User)]);
        }

        var model = new ExternalLoginsViewModel
        {
            CurrentLogins = await userManager.GetLoginsAsync(user)
        };

        model.OtherLogins = (await signInManager.GetExternalAuthenticationSchemesAsync())
            .Where(auth => model.CurrentLogins.All(ul => auth.Name != ul.LoginProvider))
            .ToList();

        model.ShowRemoveButton = await userManager.HasPasswordAsync(user) || model.CurrentLogins.Count > 1;
        model.StatusMessage = StatusMessage;

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LinkLogin(string provider)
    {
        // Clear the existing external cookie to ensure a clean login process
        await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

        // Request a redirect to the external login provider to link a login for the current user
        var redirectUrl = Url.Action(nameof(LinkLoginCallback));
        var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl, userManager.GetUserId(User));

        return new ChallengeResult(provider, properties);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GenerateRecoveryCodes()
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound(localizer["UserNotFound", userManager.GetUserId(User)]);
        }

        if (!user.TwoFactorEnabled)
        {
            AddError(localizer["ErrorGenerateCodesWithout2FA"]);
            return View();
        }

        var recoveryCodes = await userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);

        logger.LogInformation(localizer["UserGenerated2FACodes", user.Id]);

        var model = new ShowRecoveryCodesViewModel { RecoveryCodes = recoveryCodes.ToArray() };

        return View(nameof(ShowRecoveryCodes), model);
    }

    [HttpGet]
    public IActionResult ShowRecoveryCodes()
    {
        var recoveryCodes = (string[])TempData[RecoveryCodesKey];
        if (recoveryCodes == null)
        {
            return RedirectToAction(nameof(TwoFactorAuthentication));
        }

        var model = new ShowRecoveryCodesViewModel { RecoveryCodes = recoveryCodes };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> TwoFactorAuthentication()
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound(localizer["UserNotFound", userManager.GetUserId(User)]);
        }

        var model = new TwoFactorAuthenticationViewModel
        {
            HasAuthenticator = await userManager.GetAuthenticatorKeyAsync(user) != null,
            Is2faEnabled = user.TwoFactorEnabled,
            RecoveryCodesLeft = await userManager.CountRecoveryCodesAsync(user),
            IsMachineRemembered = await signInManager.IsTwoFactorClientRememberedAsync(user)
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> ForgetTwoFactorClient()
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound(localizer["UserNotFound", userManager.GetUserId(User)]);
        }

        await signInManager.ForgetTwoFactorClientAsync();

        StatusMessage = localizer["SuccessForgetBrowser2FA"];

        return RedirectToAction(nameof(TwoFactorAuthentication));
    }

    [HttpGet]
    public async Task<IActionResult> Disable2faWarning()
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound(localizer["UserNotFound", userManager.GetUserId(User)]);
        }

        if (!user.TwoFactorEnabled)
        {
            throw new ApplicationException(localizer["ErrorDisable2FA", user.Id]);
        }

        return View(nameof(Disable2fa));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Disable2fa()
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound(localizer["UserNotFound", userManager.GetUserId(User)]);
        }

        var disable2faResult = await userManager.SetTwoFactorEnabledAsync(user, false);
        if (!disable2faResult.Succeeded)
        {
            throw new ApplicationException(localizer["ErrorDisable2FA", user.Id]);
        }

        logger.LogInformation(localizer["SuccessDisabled2FA", user.Id]);

        return RedirectToAction(nameof(TwoFactorAuthentication));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetAuthenticator()
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound(localizer["UserNotFound", userManager.GetUserId(User)]);
        }

        await userManager.SetTwoFactorEnabledAsync(user, false);
        await userManager.ResetAuthenticatorKeyAsync(user);
        logger.LogInformation(localizer["SuccessResetAuthenticationKey", user.Id]);

        return RedirectToAction(nameof(EnableAuthenticator));
    }

    [HttpGet]
    public IActionResult ResetAuthenticatorWarning()
    {
        return View(nameof(ResetAuthenticator));
    }

    [HttpGet]
    public async Task<IActionResult> EnableAuthenticator()
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound(localizer["UserNotFound", userManager.GetUserId(User)]);
        }

        var model = new EnableAuthenticatorViewModel();
        await LoadSharedKeyAndQrCodeUriAsync(user, model);

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EnableAuthenticator(EnableAuthenticatorViewModel model)
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound(localizer["UserNotFound", userManager.GetUserId(User)]);
        }

        if (!ModelState.IsValid)
        {
            await LoadSharedKeyAndQrCodeUriAsync(user, model);
            return View(model);
        }

        var verificationCode = model.Code.Replace(" ", string.Empty).Replace("-", string.Empty);

        var is2faTokenValid = await userManager.VerifyTwoFactorTokenAsync(
            user, userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);

        if (!is2faTokenValid)
        {
            ModelState.AddModelError(localizer["ErrorCode"], localizer["InvalidVerificationCode"]);
            await LoadSharedKeyAndQrCodeUriAsync(user, model);
            return View(model);
        }

        await userManager.SetTwoFactorEnabledAsync(user, true);
        var userId = await userManager.GetUserIdAsync(user);

        logger.LogInformation(localizer["SuccessUserEnabled2FA"], userId);

        StatusMessage = localizer["AuthenticatorVerified"];

        if (await userManager.CountRecoveryCodesAsync(user) == 0)
        {
            var recoveryCodes = await userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
            TempData[RecoveryCodesKey] = recoveryCodes.ToArray();

            return RedirectToAction(nameof(ShowRecoveryCodes));
        }

        return RedirectToAction(nameof(TwoFactorAuthentication));
    }

    [HttpGet]
    public async Task<IActionResult> GenerateRecoveryCodesWarning()
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound(localizer["UserNotFound", userManager.GetUserId(User)]);
        }

        if (!user.TwoFactorEnabled)
        {
            throw new ApplicationException(localizer["Error2FANotEnabled", user.Id]);
        }

        return View(nameof(GenerateRecoveryCodes));
    }

    private async Task LoadSharedKeyAndQrCodeUriAsync(TUser user, EnableAuthenticatorViewModel model)
    {
        var unformattedKey = await userManager.GetAuthenticatorKeyAsync(user);
        if (string.IsNullOrEmpty(unformattedKey))
        {
            await userManager.ResetAuthenticatorKeyAsync(user);
            unformattedKey = await userManager.GetAuthenticatorKeyAsync(user);
        }

        model.SharedKey = FormatKey(unformattedKey);
        model.AuthenticatorUri = GenerateQrCodeUri(user.Email, unformattedKey);
    }

    private async Task<IndexViewModel> BuildManageIndexViewModelAsync(TUser user)
    {
        var claims = await userManager.GetClaimsAsync(user);
        var profile = OpenIdClaimHelpers.ExtractProfileInfo(claims);

        var model = new IndexViewModel
        {
            Username = user.UserName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            IsEmailConfirmed = user.EmailConfirmed,
            StatusMessage = StatusMessage,
            Name = profile.FullName,
            Website = profile.Website,
            Profile = profile.Profile,
            Country = profile.Country,
            Region = profile.Region,
            PostalCode = profile.PostalCode,
            Locality = profile.Locality,
            StreetAddress = profile.StreetAddress
        };
        return model;
    }

    private async Task UpdateUserClaimsAsync(IndexViewModel model, TUser user)
    {
        var claims = await userManager.GetClaimsAsync(user);
        var oldProfile = OpenIdClaimHelpers.ExtractProfileInfo(claims);
        var newProfile = new OpenIdProfile
        {
            Website = model.Website,
            StreetAddress = model.StreetAddress,
            Locality = model.Locality,
            PostalCode = model.PostalCode,
            Region = model.Region,
            Country = model.Country,
            FullName = model.Name,
            Profile = model.Profile
        };

        var claimsToRemove = OpenIdClaimHelpers.ExtractClaimsToRemove(oldProfile, newProfile);
        var claimsToAdd = OpenIdClaimHelpers.ExtractClaimsToAdd(oldProfile, newProfile);
        var claimsToReplace = OpenIdClaimHelpers.ExtractClaimsToReplace(claims, newProfile);

        await userManager.RemoveClaimsAsync(user, claimsToRemove);
        await userManager.AddClaimsAsync(user, claimsToAdd);

        foreach (var pair in claimsToReplace)
        {
            await userManager.ReplaceClaimAsync(user, pair.Item1, pair.Item2);
        }
    }

    private static string FormatKey(string unformattedKey)
    {
        var result = new StringBuilder();
        var currentPosition = 0;

        while (currentPosition + 4 < unformattedKey.Length)
        {
            result.Append(unformattedKey.AsSpan(currentPosition, 4)).Append(' ');
            currentPosition += 4;
        }

        if (currentPosition < unformattedKey.Length)
        {
            result.Append(unformattedKey.AsSpan(currentPosition));
        }

        return result.ToString().ToLowerInvariant();
    }

    private string GenerateQrCodeUri(string email, string unformattedKey)
    {
        return string.Format(
            AuthenticatorUriFormat,
            urlEncoder.Encode("Skoruba.Duende.IdentityServer.STS.Identity"),
            urlEncoder.Encode(email),
            unformattedKey);
    }

    private void AddErrors(IdentityResult result)
    {
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
    }

    private void AddError(string description, string title = "")
    {
        ModelState.AddModelError(title, description);
    }
}
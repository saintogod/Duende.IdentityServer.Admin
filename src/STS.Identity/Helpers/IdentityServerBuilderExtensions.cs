using System.Security.Cryptography.X509Certificates;

using Skoruba.Duende.IdentityServer.Shared.Configuration.Configuration.Common;

namespace Skoruba.Duende.IdentityServer.STS.Identity.Helpers;

public static class IdentityServerBuilderExtensions
{
    private const string certificateNotFound = "Certificate not found";
    private const string signingCertificateThumbprintNotFound = "Signing certificate thumbprint not found";
    private const string signingCertificatePathIsNotSpecified = "Signing certificate file path is not specified";

    private const string validationCertificateThumbprintNotFound = "Validation certificate thumbprint not found";
    private const string validationCertificatePathIsNotSpecified = "Validation certificate file path is not specified";

    /// <summary>
    /// Add custom signing certificate from certification store according thumbprint or from file
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IIdentityServerBuilder AddCustomSigningCredential(this IIdentityServerBuilder builder, IConfiguration configuration)
    {
        var certificateConfiguration = configuration.GetSection(nameof(CertificateConfiguration)).Get<CertificateConfiguration>();

        if (certificateConfiguration.UseSigningCertificateThumbprint)
        {
            if (string.IsNullOrWhiteSpace(certificateConfiguration.SigningCertificateThumbprint))
            {
                throw new Exception(signingCertificateThumbprintNotFound);
            }

            StoreLocation storeLocation = StoreLocation.LocalMachine;
            bool validOnly = certificateConfiguration.CertificateValidOnly;

            // Parse the Certificate StoreLocation
            string certStoreLocationLower = certificateConfiguration.CertificateStoreLocation.ToLower();
            if (certStoreLocationLower == StoreLocation.CurrentUser.ToString().ToLower() ||
                certificateConfiguration.CertificateStoreLocation == ((int)StoreLocation.CurrentUser).ToString())
            {
                storeLocation = StoreLocation.CurrentUser;
            }
            else if (certStoreLocationLower == StoreLocation.LocalMachine.ToString().ToLower() ||
                     certStoreLocationLower == ((int)StoreLocation.LocalMachine).ToString())
            {
                storeLocation = StoreLocation.LocalMachine;
            }
            else
            {
                storeLocation = StoreLocation.LocalMachine;
                validOnly = true;
            }

            // Open Certificate
            using var certStore = new X509Store(StoreName.My, storeLocation);
            certStore.Open(OpenFlags.ReadOnly);

            var certCollection = certStore.Certificates.Find(X509FindType.FindByThumbprint, certificateConfiguration.SigningCertificateThumbprint, validOnly);
            if (certCollection.Count == 0)
            {
                throw new Exception(certificateNotFound);
            }

            var certificate = certCollection[0];

            return builder.AddSigningCredential(certificate);
        }

        if (certificateConfiguration.UseSigningCertificatePfxFile)
        {
            if (string.IsNullOrWhiteSpace(certificateConfiguration.SigningCertificatePfxFilePath))
            {
                throw new Exception(signingCertificatePathIsNotSpecified);
            }

            if (!File.Exists(certificateConfiguration.SigningCertificatePfxFilePath))
            {
                throw new Exception($"Signing key file: {certificateConfiguration.SigningCertificatePfxFilePath} not found");
            }
            try
            {
                return builder.AddSigningCredential(new X509Certificate2(certificateConfiguration.SigningCertificatePfxFilePath, certificateConfiguration.SigningCertificatePfxFilePassword));
            }
            catch (Exception e)
            {
                throw new Exception("There was an error adding the key file - during the creation of the signing key", e);
            }
        }

        if (certificateConfiguration.UseTemporarySigningKeyForDevelopment)
        {
            return builder.AddDeveloperSigningCredential();
        }
        throw new Exception("Signing credential is not specified");
    }

    /// <summary>
    /// Add custom validation key for signing key rollover
    /// http://docs.identityserver.io/en/latest/topics/crypto.html#signing-key-rollover
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IIdentityServerBuilder AddCustomValidationKey(this IIdentityServerBuilder builder, IConfiguration configuration)
    {
        var certificateConfiguration = configuration.GetSection(nameof(CertificateConfiguration)).Get<CertificateConfiguration>();

        if (certificateConfiguration.UseValidationCertificateThumbprint)
        {
            if (string.IsNullOrWhiteSpace(certificateConfiguration.ValidationCertificateThumbprint))
            {
                throw new Exception(validationCertificateThumbprintNotFound);
            }

            using var certStore = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            certStore.Open(OpenFlags.ReadOnly);

            var certCollection = certStore.Certificates.Find(X509FindType.FindByThumbprint,
                certificateConfiguration.ValidationCertificateThumbprint,
                false);
            if (certCollection.Count == 0)
            {
                throw new Exception(certificateNotFound);
            }

            var certificate = certCollection[0];

            return builder.AddValidationKey(certificate);
        }

        if (certificateConfiguration.UseValidationCertificatePfxFile)
        {
            if (string.IsNullOrWhiteSpace(certificateConfiguration.ValidationCertificatePfxFilePath))
            {
                throw new Exception(validationCertificatePathIsNotSpecified);
            }

            if (!File.Exists(certificateConfiguration.ValidationCertificatePfxFilePath))
                throw new Exception($"Validation key file: {certificateConfiguration.ValidationCertificatePfxFilePath} not found");

            try
            {
                return builder.AddValidationKey(new X509Certificate2(certificateConfiguration.ValidationCertificatePfxFilePath, certificateConfiguration.ValidationCertificatePfxFilePassword));
            }
            catch (Exception e)
            {
                throw new Exception("There was an error adding the key file - during the creation of the validation key", e);
            }
        }
        return builder;
    }
}

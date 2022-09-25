using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

using SendGrid;

using Skoruba.Duende.IdentityServer.Shared.Configuration.Configuration.Email;
using Skoruba.Duende.IdentityServer.Shared.Configuration.Email;

namespace Microsoft.Extensions.DependencyInjection;

public static class EmailSenderExtensions
{
    /// <summary>
    /// Add email senders - configuration of sendgrid, smtp senders
    /// </summary>
    public static void AddEmailSenders(this IServiceCollection services, IConfiguration configuration)
    {
        var smtpConfiguration = configuration.GetSection(nameof(SmtpConfiguration)).Get<SmtpConfiguration>();
        var sendGridConfiguration = configuration.GetSection(nameof(SendGridConfiguration)).Get<SendGridConfiguration>();

        if (!string.IsNullOrWhiteSpace(sendGridConfiguration?.ApiKey))
        {
            services.AddSingleton(sendGridConfiguration);
            services.AddSingleton<ISendGridClient>(_ => new SendGridClient(sendGridConfiguration.ApiKey));
            services.AddTransient<IEmailSender, SendGridEmailSender>();
        }
        else if (!string.IsNullOrWhiteSpace(smtpConfiguration?.Host))
        {
            services.AddSingleton(smtpConfiguration);
            services.AddTransient<IEmailSender, SmtpEmailSender>();
        }
        else
        {
            services.AddSingleton<IEmailSender, LogEmailSender>();
        }
    }

    public static void AddDataProtection<TDbContext>(this IServiceCollection services)
        where TDbContext : DbContext, IDataProtectionKeyContext
    {
        services.AddDataProtection()
            .SetApplicationName("Saintogod.Ids")
            .PersistKeysToDbContext<TDbContext>();
    }
}

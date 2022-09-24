using HealthChecks.UI.Client;

using Skoruba.Duende.IdentityServer.Admin.EntityFramework.Shared.DbContexts;
using Skoruba.Duende.IdentityServer.Admin.EntityFramework.Shared.Entities.Identity;
using Skoruba.Duende.IdentityServer.STS.Identity.Configuration;
using Skoruba.Duende.IdentityServer.STS.Identity.Configuration.Constants;

namespace Skoruba.Duende.IdentityServer.STS.Identity;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        var rootConfiguration = CreateRootConfiguration();
        services.AddSingleton(rootConfiguration);
        // Register DbContexts for IdentityServer and Identity
        RegisterDbContexts(services);

        // Save data protection keys to db, using a common application name shared between Admin and STS
        services.AddDataProtection<IdentityServerDataProtectionDbContext>();

        // Add email senders which is currently setup for SendGrid and SMTP
        services.AddEmailSenders(Configuration);

        // Add services for authentication, including Identity model and external providers
        services.AddAuthenticationServices<AdminIdentityDbContext, UserIdentity, UserIdentityRole>(Configuration);
        services.AddIdentityServer<IdentityServerConfigurationDbContext, IdentityServerPersistedGrantDbContext, UserIdentity>(Configuration);

        // Add HSTS options
        services.AddHsts(options =>
        {
            options.Preload = true;
            options.IncludeSubDomains = true;
            options.MaxAge = TimeSpan.FromDays(365);
        });

        // Add all dependencies for Asp.Net Core Identity in MVC - these dependencies are injected into generic Controllers
        // Including settings for MVC and Localization
        // If you want to change primary keys or use another db model for Asp.Net Core Identity:
        services.AddMvcWithLocalization<UserIdentity, string>(Configuration);

        // Add authorization policies for MVC
        services.AddAuthorizationPolicies(rootConfiguration);

        services.AddIdSHealthChecks<IdentityServerConfigurationDbContext, IdentityServerPersistedGrantDbContext, AdminIdentityDbContext, IdentityServerDataProtectionDbContext>(Configuration);
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseCookiePolicy();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseHsts();
        }

        app.UsePathBase(Configuration.GetValue<string>("BasePath"));

        app.UseStaticFiles();
        app.UseIdentityServer();

        // Add custom security headers
        app.UseSecurityHeaders(Configuration);

        app.UseMvcLocalizationServices();

        app.UseRouting();
        app.UseAuthorization();
        app.UseEndpoints(endpoint =>
        {
            endpoint.MapDefaultControllerRoute();
            endpoint.MapHealthChecks("/health", new() { ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse });
        });
    }

    public virtual void RegisterDbContexts(IServiceCollection services)
    {
        services.RegisterDbContexts<AdminIdentityDbContext, IdentityServerConfigurationDbContext, IdentityServerPersistedGrantDbContext, IdentityServerDataProtectionDbContext>(Configuration);
    }

    protected IRootConfiguration CreateRootConfiguration()
    {
        var rootConfiguration = new RootConfiguration();
        Configuration.GetSection(ConfigurationConsts.AdminConfigurationKey).Bind(rootConfiguration.AdminConfiguration);
        Configuration.GetSection(ConfigurationConsts.RegisterConfigurationKey).Bind(rootConfiguration.RegisterConfiguration);
        return rootConfiguration;
    }
}

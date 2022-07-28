using System.Reflection;

using Microsoft.Extensions.FileProviders;

using Skoruba.Duende.IdentityServer.Admin.UI.Helpers.ApplicationBuilder;

namespace Skoruba.Duende.IdentityServer.Admin.Helpers;

public static class StartupHelpers
{
    public static void AddAdminUIRazorRuntimeCompilation(this IServiceCollection services, IWebHostEnvironment hostingEnvironment)
    {
        if (hostingEnvironment.IsDevelopment())
        {
            var builder = services.AddControllersWithViews();

            var adminAssembly = typeof(AdminUIApplicationBuilderExtensions).GetTypeInfo().Assembly.GetName().Name;

            builder.AddRazorRuntimeCompilation(options =>
            {
                if (adminAssembly == null) return;

                var libraryPath = Path.GetFullPath(Path.Combine(hostingEnvironment.ContentRootPath, "..", adminAssembly));

                if (Directory.Exists(libraryPath))
                {
                    options.FileProviders.Add(new PhysicalFileProvider(libraryPath));
                }
            });
        }
    }
}
using Microsoft.Extensions.Hosting;

namespace Microsoft.Extensions.Configuration;

public static class GlobalConfigurationHelper
{
    public static IConfiguration GetConfiguration<TStartup>(string[] args) where TStartup: class
    {
        var configBuilder = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory);

        configBuilder.ResetProviders<TStartup>(args);
        return configBuilder.Build();
    }

    public static IConfigurationBuilder ResetProviders<TStartup>(this IConfigurationBuilder configBuilder, string[] args, params string[] additionalFiles) where TStartup : class
    {
        configBuilder.Sources.Clear();

        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        var isDevelopment = environment == Environments.Development;

        configBuilder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
            .AddCommonConfig(additionalFiles);

        if (isDevelopment)
        {
            configBuilder.AddUserSecrets<TStartup>(true);
        }

        configBuilder.AddEnvironmentVariables();
        configBuilder.AddCommandLine(args);

        return configBuilder;
    }

    public static IConfigurationBuilder AddCommonConfig(this IConfigurationBuilder configBuilder, params string[] additionalFiles)
    {
        var configFolder = Environment.GetEnvironmentVariable("APPCONFIGFOLDER");
        if (string.IsNullOrEmpty(configFolder))
        {
            configFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "st.ids");
        }
        if (!Directory.Exists(configFolder))
        {
            try
            {
                Directory.CreateDirectory(configFolder);
            }
            catch
            { }
        }
        Console.WriteLine($"Using AppFolder at: {configFolder}");
        configBuilder.AddJsonFile("serilog.json", optional: true, reloadOnChange: true)
            .AddJsonFile(Path.Combine(configFolder, "serilog.json"), optional: true, reloadOnChange: true)
            .AddJsonFile(Path.Combine(configFolder, "deployment.json"), optional: true, reloadOnChange: true);
        
        foreach (var additionalFile in additionalFiles)
            configBuilder.AddJsonFile(Path.Combine(configFolder, additionalFile), optional: true, reloadOnChange: false);

        return configBuilder;
    }
}

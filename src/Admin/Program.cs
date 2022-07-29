// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using Serilog;

using Skoruba.Duende.IdentityServer.Admin;

var configuration = GlobalConfigurationHelper.GetConfiguration<Startup>(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateLogger();

try
{
    await CreateHostBuilder(args)
        .Build()
        .RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

static IHostBuilder CreateHostBuilder(string[] args) =>
   Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration((hostContext, configApp) =>
        {
            configApp.ResetProviders<Startup>(args);
            configApp.AddJsonFile("identitydata.json", optional: true, reloadOnChange: true);
            configApp.AddJsonFile("identityserverdata.json", optional: true, reloadOnChange: true);

        })
       .ConfigureWebHostDefaults(webBuilder =>
       {
           webBuilder.ConfigureKestrel(options => options.AddServerHeader = false);
           webBuilder.UseStartup<Startup>();
       })
       .UseSerilog((hostContext, loggerConfig) =>
       {
           loggerConfig
               .ReadFrom.Configuration(hostContext.Configuration)
               .Enrich.WithProperty("ApplicationName", hostContext.HostingEnvironment.ApplicationName);
       });

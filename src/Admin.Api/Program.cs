// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using Serilog;

using Skoruba.Duende.IdentityServer.Admin.Api;

var configuration = GlobalConfigurationHelper.GetConfiguration<Startup>(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateLogger();
try
{
    Host.CreateDefaultBuilder(args)
       .ConfigureAppConfiguration((hostContext, configApp) =>
       {
           configApp.ResetProviders<Startup>(args);
       })
       .ConfigureLogging((context, logging) =>
       {
           logging.AddSerilog();
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
      })
      .Build()
      .Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

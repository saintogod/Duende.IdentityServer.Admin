# How to create database

```bash

doco run --no-deps  --rm -e SeedConfiguration__ApplySeed=true -e DatabaseMigrationsConfiguration__ApplyDatabaseMigrations=true -e MigrateOnly=true ids.admin dotnet Skoruba.Duende.IdentityServer.Admin.Api.dll
```

```pwsh
dotnet run -c Debug --no-restore -- SeedConfiguration:ApplySeed=true -e DatabaseMigrationsConfiguration:ApplyDatabaseMigrations=true -e MigrateOnly=true

```
# Dotnt Ef Core Migrations

## Update Migrations

```pwsh

dotnet ef migrations add DbInit --no-build --configuration Debug -p ..\EntityFramework.MySql\EntityFramework.MySql.csproj -c AdminAuditLogDbContext -o Migrations\AuditLogging

dotnet ef migrations add DbInit --no-build --configuration Debug -p ..\EntityFramework.MySql\EntityFramework.MySql.csproj -c IdentityServerDataProtectionDbContext -o Migrations\DataProtection

dotnet ef migrations add DbInit --no-build --configuration Debug -p ..\EntityFramework.MySql\EntityFramework.MySql.csproj -c AdminIdentityDbContext -o Migrations\Identity

dotnet ef migrations add DbInit --no-build --configuration Debug -p ..\EntityFramework.MySql\EntityFramework.MySql.csproj -c IdentityServerConfigurationDbContext -o Migrations\IdentityServerConfiguration

dotnet ef migrations add DbInit --no-build --configuration Debug -p ..\EntityFramework.MySql\EntityFramework.MySql.csproj -c IdentityServerPersistedGrantDbContext -o Migrations\IdentityServerGrants

dotnet ef migrations add DbInit --no-build --configuration Debug -p ..\EntityFramework.MySql\EntityFramework.MySql.csproj -c AdminLogDbContext -o Migrations\Logging

```

## Update Database

```pwsh

dotnet ef database update --no-build --configuration Debug -p ..\EntityFramework.MySql\EntityFramework.MySql.csproj -c AdminAuditLogDbContext

dotnet ef database update --no-build --configuration Debug -p ..\EntityFramework.MySql\EntityFramework.MySql.csproj -c IdentityServerDataProtectionDbContext

dotnet ef database update --no-build --configuration Debug -p ..\EntityFramework.MySql\EntityFramework.MySql.csproj -c AdminIdentityDbContext

dotnet ef database update --no-build --configuration Debug -p ..\EntityFramework.MySql\EntityFramework.MySql.csproj -c IdentityServerConfigurationDbContext

dotnet ef database update --no-build --configuration Debug -p ..\EntityFramework.MySql\EntityFramework.MySql.csproj -c IdentityServerPersistedGrantDbContext

dotnet ef database update --no-build --configuration Debug -p ..\EntityFramework.MySql\EntityFramework.MySql.csproj -c AdminLogDbContext

```

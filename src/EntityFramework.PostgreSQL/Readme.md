# Dotnet Ef Core Migrations for PostgreSQL

```pwsh

dotnet ef migrations add DbInit --no-build --configuration Debug -p ..\EntityFramework.PostgreSQL\EntityFramework.PostgreSQL.csproj -c AdminAuditLogDbContext -o Migrations\AuditLogging

dotnet ef migrations add DbInit --no-build --configuration Debug -p ..\EntityFramework.PostgreSQL\EntityFramework.PostgreSQL.csproj -c IdentityServerDataProtectionDbContext -o Migrations\DataProtection

dotnet ef migrations add DbInit --no-build --configuration Debug -p ..\EntityFramework.PostgreSQL\EntityFramework.PostgreSQL.csproj -c AdminIdentityDbContext -o Migrations\Identity

dotnet ef migrations add DbInit --no-build --configuration Debug -p ..\EntityFramework.PostgreSQL\EntityFramework.PostgreSQL.csproj -c IdentityServerConfigurationDbContext -o Migrations\IdentityServerConfiguration

dotnet ef migrations add DbInit --no-build --configuration Debug -p ..\EntityFramework.PostgreSQL\EntityFramework.PostgreSQL.csproj -c IdentityServerPersistedGrantDbContext -o Migrations\IdentityServerGrants

dotnet ef migrations add DbInit --no-build --configuration Debug -p ..\EntityFramework.PostgreSQL\EntityFramework.PostgreSQL.csproj -c AdminLogDbContext -o Migrations\Logging

```

## Update database

```pwsh

dotnet ef database update --no-build --configuration Debug -p ..\EntityFramework.PostgreSQL\EntityFramework.PostgreSQL.csproj -c AdminAuditLogDbContext

dotnet ef database update --no-build --configuration Debug -p ..\EntityFramework.PostgreSQL\EntityFramework.PostgreSQL.csproj -c IdentityServerDataProtectionDbContext

dotnet ef database update --no-build --configuration Debug -p ..\EntityFramework.PostgreSQL\EntityFramework.PostgreSQL.csproj -c AdminIdentityDbContext

dotnet ef database update --no-build --configuration Debug -p ..\EntityFramework.PostgreSQL\EntityFramework.PostgreSQL.csproj -c IdentityServerConfigurationDbContext

dotnet ef database update --no-build --configuration Debug -p ..\EntityFramework.PostgreSQL\EntityFramework.PostgreSQL.csproj -c IdentityServerPersistedGrantDbContext

dotnet ef database update --no-build --configuration Debug -p ..\EntityFramework.PostgreSQL\EntityFramework.PostgreSQL.csproj -c AdminLogDbContext

```

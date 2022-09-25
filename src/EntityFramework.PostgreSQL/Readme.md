# Dotnet Ef Core Migrations for PostgreSQL

```pwsh
# At Admin/ Folder.
$ProjectName="EntityFramework.PostgreSQL"
dotnet ef migrations add DbInit --no-build --configuration Debug -p ..\$ProjectName\$ProjectName.csproj -c AdminAuditLogDbContext -o Migrations\AuditLogging; `
dotnet ef migrations add DbInit --no-build --configuration Debug -p ..\$ProjectName\$ProjectName.csproj -c IdentityServerDataProtectionDbContext -o Migrations\DataProtection; `
dotnet ef migrations add DbInit --no-build --configuration Debug -p ..\$ProjectName\$ProjectName.csproj -c AdminIdentityDbContext -o Migrations\Identity; `
dotnet ef migrations add DbInit --no-build --configuration Debug -p ..\$ProjectName\$ProjectName.csproj -c IdentityServerConfigurationDbContext -o Migrations\IdentityServerConfiguration; `
dotnet ef migrations add DbInit --no-build --configuration Debug -p ..\$ProjectName\$ProjectName.csproj -c IdentityServerPersistedGrantDbContext -o Migrations\IdentityServerGrants; `
dotnet ef migrations add DbInit --no-build --configuration Debug -p ..\$ProjectName\$ProjectName.csproj -c AdminLogDbContext -o Migrations\Logging;

```

## Update database

```pwsh

dotnet ef database update --no-build --configuration Debug -p ..\$ProjectName\$ProjectName.csproj -c AdminAuditLogDbContext; `
dotnet ef database update --no-build --configuration Debug -p ..\$ProjectName\$ProjectName.csproj -c IdentityServerDataProtectionDbContext; `
dotnet ef database update --no-build --configuration Debug -p ..\$ProjectName\$ProjectName.csproj -c AdminIdentityDbContext; `
dotnet ef database update --no-build --configuration Debug -p ..\$ProjectName\$ProjectName.csproj -c IdentityServerConfigurationDbContext; `
dotnet ef database update --no-build --configuration Debug -p ..\$ProjectName\$ProjectName.csproj -c IdentityServerPersistedGrantDbContext; `
dotnet ef database update --no-build --configuration Debug -p ..\$ProjectName\$ProjectName.csproj -c AdminLogDbContext;

```

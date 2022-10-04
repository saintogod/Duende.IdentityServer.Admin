# How to start this project
 
```powershell
# Create data base and apply seed.
dotnet run -c Debug --no-build --nologo -- --MigrateOnly true --ApplyMigrations true --ApplySeed true
```
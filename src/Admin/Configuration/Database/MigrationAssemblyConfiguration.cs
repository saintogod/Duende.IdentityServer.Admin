﻿// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System.Reflection;

using Skoruba.Duende.IdentityServer.Admin.EntityFramework.Configuration.Configuration;

using MySqlMigrationAssembly = Skoruba.Duende.IdentityServer.Admin.EntityFramework.MySql.Helpers.MigrationAssembly;
using PostgreSQLMigrationAssembly = Skoruba.Duende.IdentityServer.Admin.EntityFramework.PostgreSQL.Helpers.MigrationAssembly;

namespace Skoruba.Duende.IdentityServer.Admin.Configuration.Database;

public static class MigrationAssemblyConfiguration
{
    public static string GetMigrationAssemblyByProvider(this DatabaseProviderConfiguration databaseProvider)
    {
        return databaseProvider.ProviderType switch
        {
            DatabaseProviderType.PostgreSQL => typeof(PostgreSQLMigrationAssembly).GetTypeInfo().Assembly.GetName().Name,
            DatabaseProviderType.MySql => typeof(MySqlMigrationAssembly).GetTypeInfo().Assembly.GetName().Name,
            _ => throw new ArgumentOutOfRangeException(nameof(databaseProvider.ProviderType))
        };
    }
}
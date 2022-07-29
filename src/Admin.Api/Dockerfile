FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base

EXPOSE 443

COPY share/cacerts.crt /usr/local/share/ca-certificates
RUN update-ca-certificates

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

COPY Directory.Build.props ./
COPY src/Admin.Api/Admin.Api.csproj Admin.Api/
COPY src/Admin.BusinessLogic.Identity/Admin.BusinessLogic.Identity.csproj Admin.BusinessLogic.Identity/
COPY src/Admin.BusinessLogic.Shared/Admin.BusinessLogic.Shared.csproj Admin.BusinessLogic.Shared/
COPY src/Admin.BusinessLogic/Admin.BusinessLogic.csproj Admin.BusinessLogic/
COPY src/Admin.EntityFramework.Configuration/Admin.EntityFramework.Configuration.csproj Admin.EntityFramework.Configuration/
COPY src/Admin.EntityFramework.Extensions/Admin.EntityFramework.Extensions.csproj Admin.EntityFramework.Extensions/
COPY src/Admin.EntityFramework.Identity/Admin.EntityFramework.Identity.csproj Admin.EntityFramework.Identity/
COPY src/Admin.EntityFramework.MySql/Admin.EntityFramework.MySql.csproj Admin.EntityFramework.MySql/
COPY src/Admin.EntityFramework.PostgreSQL/Admin.EntityFramework.PostgreSQL.csproj Admin.EntityFramework.PostgreSQL/
COPY src/Admin.EntityFramework.Shared/Admin.EntityFramework.Shared.csproj Admin.EntityFramework.Shared/
COPY src/Admin.EntityFramework.SqlServer/Admin.EntityFramework.SqlServer.csproj Admin.EntityFramework.SqlServer/
COPY src/Admin.EntityFramework/Admin.EntityFramework.csproj Admin.EntityFramework/
COPY src/Shared.Configuration/Shared.Configuration.csproj Shared.Configuration/
COPY src/Shared/Shared.csproj Shared/

RUN dotnet restore Admin.Api/Admin.Api.csproj

COPY src/ ./

WORKDIR /src/Admin.Api
RUN dotnet build Admin.Api.csproj -c Release -o /app/build --no-restore

FROM build AS publish
RUN dotnet publish Admin.Api.csproj -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_FORWARDEDHEADERS_ENABLED=true
ENTRYPOINT ["Skoruba.Duende.IdentityServer.Admin.Api.exe"]
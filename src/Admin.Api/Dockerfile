FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base

EXPOSE 80
EXPOSE 443
COPY share/cacerts.crt /usr/local/share/ca-certificates
RUN update-ca-certificates

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

COPY ["Directory.Build.props", "src/"]
COPY ["src/Admin.Api/Admin.Api.csproj", "src/Admin.Api/"]
COPY ["src/Admin.BusinessLogic.Identity/Admin.BusinessLogic.Identity.csproj", "src/Admin.BusinessLogic.Identity/"]
COPY ["src/Admin.BusinessLogic.Shared/Admin.BusinessLogic.Shared.csproj", "src/Admin.BusinessLogic.Shared/"]
COPY ["src/Admin.BusinessLogic/Admin.BusinessLogic.csproj", "src/Admin.BusinessLogic/"]
COPY ["src/Admin.EntityFramework.Configuration/Admin.EntityFramework.Configuration.csproj", "src/Admin.EntityFramework.Configuration/"]
COPY ["src/Admin.EntityFramework.Extensions/Admin.EntityFramework.Extensions.csproj", "src/Admin.EntityFramework.Extensions/"]
COPY ["src/Admin.EntityFramework.Identity/Admin.EntityFramework.Identity.csproj", "src/Admin.EntityFramework.Identity/"]
COPY ["src/Admin.EntityFramework.MySql/Admin.EntityFramework.MySql.csproj", "src/Admin.EntityFramework.MySql/"]
COPY ["src/Admin.EntityFramework.PostgreSQL/Admin.EntityFramework.PostgreSQL.csproj", "src/Admin.EntityFramework.PostgreSQL/"]
COPY ["src/Admin.EntityFramework.Shared/Admin.EntityFramework.Shared.csproj", "src/Admin.EntityFramework.Shared/"]
COPY ["src/Admin.EntityFramework.SqlServer/Admin.EntityFramework.SqlServer.csproj", "src/Admin.EntityFramework.SqlServer/"]
COPY ["src/Admin.EntityFramework/Admin.EntityFramework.csproj", "src/Admin.EntityFramework/"]
COPY ["src/Shared.Configuration/Shared.Configuration.csproj", "src/Shared.Configuration/"]
COPY ["src/Shared/Shared.csproj", "src/Shared/"]

RUN dotnet restore "src/Admin.Api/Admin.Api.csproj"
COPY . .
WORKDIR "/src/src/Admin.Api"
RUN dotnet build "Admin.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Admin.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_FORWARDEDHEADERS_ENABLED=true
ENTRYPOINT ["dotnet", "Skoruba.Duende.IdentityServer.Admin.Api.dll"]
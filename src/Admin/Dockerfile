FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base

EXPOSE 80
EXPOSE 443
COPY share/cacerts.crt /usr/local/share/ca-certificates
RUN update-ca-certificates

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

COPY ["Directory.Build.props", "src/"]
COPY ["src/Admin.UI/Admin.UI.csproj", "src/Admin.UI/"]
COPY ["src/Admin/Admin.csproj", "src/Admin/"]
COPY ["src/BusinessLogic.Identity/BusinessLogic.Identity.csproj", "src/BusinessLogic.Identity/"]
COPY ["src/BusinessLogic.Shared/BusinessLogic.Shared.csproj", "src/BusinessLogic.Shared/"]
COPY ["src/BusinessLogic/BusinessLogic.csproj", "src/BusinessLogic/"]
COPY ["src/EntityFramework.Configuration/EntityFramework.Configuration.csproj", "src/EntityFramework.Configuration/"]
COPY ["src/EntityFramework.Extensions/EntityFramework.Extensions.csproj", "src/EntityFramework.Extensions/"]
COPY ["src/EntityFramework.Identity/EntityFramework.Identity.csproj", "src/EntityFramework.Identity/"]
COPY ["src/EntityFramework.MySql/EntityFramework.MySql.csproj", "src/EntityFramework.MySql/"]
COPY ["src/EntityFramework.PostgreSQL/EntityFramework.PostgreSQL.csproj", "src/EntityFramework.PostgreSQL/"]
COPY ["src/EntityFramework.Shared/EntityFramework.Shared.csproj", "src/EntityFramework.Shared/"]
COPY ["src/EntityFramework.SqlServer/EntityFramework.SqlServer.csproj", "src/EntityFramework.SqlServer/"]
COPY ["src/EntityFramework/EntityFramework.csproj", "src/EntityFramework/"]
COPY ["src/Shared.Configuration/Shared.Configuration.csproj", "src/Shared.Configuration/"]
COPY ["src/Shared/Shared.csproj", "src/Shared/"]

RUN dotnet restore "src/Admin/Admin.csproj"
COPY . .
WORKDIR "/src/src/Admin"
RUN dotnet build "Admin.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Admin.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_FORWARDEDHEADERS_ENABLED=true
ENTRYPOINT ["dotnet", "Skoruba.Duende.IdentityServer.Admin.dll"]
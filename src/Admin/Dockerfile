FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base

EXPOSE 443

COPY shared/cacerts.crt shared/dev_ca.crt /usr/local/share/ca-certificates/
RUN update-ca-certificates

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

COPY src/Directory.Build.props ./
COPY src/Admin.UI/Admin.UI.csproj Admin.UI/
COPY src/Admin/Admin.csproj Admin/
COPY src/BusinessLogic.Identity/BusinessLogic.Identity.csproj BusinessLogic.Identity/
COPY src/BusinessLogic.Shared/BusinessLogic.Shared.csproj BusinessLogic.Shared/
COPY src/BusinessLogic/BusinessLogic.csproj BusinessLogic/
COPY src/EntityFramework.Configuration/EntityFramework.Configuration.csproj EntityFramework.Configuration/
COPY src/EntityFramework.Extensions/EntityFramework.Extensions.csproj EntityFramework.Extensions/
COPY src/EntityFramework.Identity/EntityFramework.Identity.csproj EntityFramework.Identity/
COPY src/EntityFramework.MySql/EntityFramework.MySql.csproj EntityFramework.MySql/
COPY src/EntityFramework.PostgreSQL/EntityFramework.PostgreSQL.csproj EntityFramework.PostgreSQL/
COPY src/EntityFramework.Shared/EntityFramework.Shared.csproj EntityFramework.Shared/
COPY src/EntityFramework.SqlServer/EntityFramework.SqlServer.csproj EntityFramework.SqlServer/
COPY src/EntityFramework/EntityFramework.csproj EntityFramework/
COPY src/Shared.Configuration/Shared.Configuration.csproj Shared.Configuration/
COPY src/Shared/Shared.csproj Shared/

RUN dotnet restore Admin/Admin.csproj

COPY src/ ./

WORKDIR /src/Admin
RUN dotnet build Admin.csproj -c Release -o /app/build --no-restore

FROM build AS publish
RUN dotnet publish Admin.csproj -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

VOLUME ["/etc/st.ids", "/var/log/st.ids", "/usr/share/st.ids"]
ENV ASPNETCORE_FORWARDEDHEADERS_ENABLED=true
ENTRYPOINT ["dotnet", "Skoruba.Duende.IdentityServer.Admin.dll"]
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base

EXPOSE 443

COPY shared/cacerts.crt /usr/local/share/ca-certificates
RUN update-ca-certificates

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

COPY src/Directory.Build.props ./
COPY src/BusinessLogic.Identity/BusinessLogic.Identity.csproj BusinessLogic.Identity/
COPY src/BusinessLogic.Shared/BusinessLogic.Shared.csproj BusinessLogic.Shared/
COPY src/EntityFramework.Configuration/EntityFramework.Configuration.csproj EntityFramework.Configuration/
COPY src/EntityFramework.Extensions/EntityFramework.Extensions.csproj EntityFramework.Extensions/
COPY src/EntityFramework.Identity/EntityFramework.Identity.csproj EntityFramework.Identity/
COPY src/EntityFramework.Shared/EntityFramework.Shared.csproj EntityFramework.Shared/
COPY src/EntityFramework/EntityFramework.csproj EntityFramework/
COPY src/Shared.Configuration/Shared.Configuration.csproj Shared.Configuration/
COPY src/Shared/Shared.csproj Shared/
COPY src/STS.Identity/STS.Identity.csproj STS.Identity/

RUN dotnet restore STS.Identity/STS.Identity.csproj

COPY src/ ./

WORKDIR /src/STS.Identity
RUN dotnet build STS.Identity.csproj -c Release -o /app/build --no-restore

FROM build AS publish
RUN dotnet publish STS.Identity.csproj -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_FORWARDEDHEADERS_ENABLED=true
ENTRYPOINT ["dotnet", "Skoruba.Duende.IdentityServer.STS.Identity.dll"]
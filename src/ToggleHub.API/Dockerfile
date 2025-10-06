FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/ToggleHub.API/ToggleHub.API.csproj", "src/ToggleHub.API/"]
COPY ["src/ToggleHub.Application/ToggleHub.Application.csproj", "src/ToggleHub.Application/"]
COPY ["src/ToggleHub.Domain/ToggleHub.Domain.csproj", "src/ToggleHub.Domain/"]
COPY ["src/ToggleHub.Infrastructure.Identity/ToggleHub.Infrastructure.Identity.csproj", "src/ToggleHub.Infrastructure.Identity/"]
COPY ["src/ToggleHub.Infrastructure/ToggleHub.Infrastructure.csproj", "src/ToggleHub.Infrastructure/"]
RUN dotnet restore "src/ToggleHub.API/ToggleHub.API.csproj"
COPY . .
WORKDIR "/src/src/ToggleHub.API"
RUN dotnet build "./ToggleHub.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ToggleHub.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ToggleHub.API.dll"]

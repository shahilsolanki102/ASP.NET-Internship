# =============================================================================
# Multi-Stage Production Dockerfile for ASP.NET Core 9.0 Web Applications
# Optimized for minimal image size (< 120MB), high security, and fast startup.
# =============================================================================

# STAGE 1: Build & Publish Environment
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /src

# Copy project files for caching layer
COPY ["Week-3/src/OrderManagementApp/OrderManagementApp.csproj", "Week-3/src/OrderManagementApp/"]
COPY ["Week-3/src/OrderManagement.UnitTests/OrderManagement.UnitTests.csproj", "Week-3/src/OrderManagement.UnitTests/"]
COPY ["Week-3/src/OrderManagement.IntegrationTests/OrderManagement.IntegrationTests.csproj", "Week-3/src/OrderManagement.IntegrationTests/"]

# Restore NuGet packages
RUN dotnet restore "Week-3/src/OrderManagementApp/OrderManagementApp.csproj"

# Copy all source code
COPY . .

# Build and Publish in Release configuration
WORKDIR "/src/Week-3/src/OrderManagementApp"
RUN dotnet publish "OrderManagementApp.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

# STAGE 2: Lightweight Production Runtime Environment
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS final
WORKDIR /app

# Install security updates & ICU globalization support
RUN apk add --no-cache icu-libs tzdata

# Configure environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
ENV TZ=UTC

# Create and switch to non-root secure user
USER $APP_UID

# Copy published binaries from build stage
COPY --from=build --chown=$APP_UID:$APP_UID /app/publish .

# Expose HTTP port
EXPOSE 8080

# Health check
HEALTHCHECK --interval=30s --timeout=5s --start-period=5s --retries=3 \
  CMD wget --no-verbose --tries=1 --spider http://localhost:8080/ || exit 1

ENTRYPOINT ["dotnet", "OrderManagementApp.dll"]

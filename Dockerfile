# Multi-stage Docker build for JWT POC API
# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["JwtPoc.sln", "./"]
COPY ["src/JwtPoc.Api/JwtPoc.Api.csproj", "src/JwtPoc.Api/"]
COPY ["src/JwtPoc.Core/JwtPoc.Core.csproj", "src/JwtPoc.Core/"]
COPY ["src/JwtPoc.Infrastructure/JwtPoc.Infrastructure.csproj", "src/JwtPoc.Infrastructure/"]

# Restore dependencies
RUN dotnet restore

# Copy source code
COPY . .

# Build the application
WORKDIR "/src/src/JwtPoc.Api"
RUN dotnet build "JwtPoc.Api.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "JwtPoc.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Create non-root user for security
RUN groupadd -r jwtpoc && useradd -r -g jwtpoc jwtpocuser

# Copy published application
COPY --from=publish /app/publish .

# Set ownership
RUN chown -R jwtpocuser:jwtpoc /app

# Switch to non-root user
USER jwtpocuser

# Expose ports
EXPOSE 8080
EXPOSE 8081

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080;https://+:8081
ENV ASPNETCORE_ENVIRONMENT=Production

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

# Run the application
ENTRYPOINT ["dotnet", "JwtPoc.Api.dll"]

# Stage 1: Build & Generate Migrations
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 1. Install dotnet-ef tool globally inside the image
# We specify the version to avoid 'DotnetToolSettings.xml' corruption errors
RUN dotnet tool install --global dotnet-ef --version 8.0.10

# Add .dotnet/tools to PATH so we can run the tool
ENV PATH="$PATH:/root/.dotnet/tools"

# 2. Copy csproj files to cache dependencies
COPY src/StarterKit.Domain/*.csproj src/StarterKit.Domain/
COPY src/StarterKit.Application/*.csproj src/StarterKit.Application/
COPY src/StarterKit.Infrastructure/*.csproj src/StarterKit.Infrastructure/
COPY src/StarterKit.Api/*.csproj src/StarterKit.Api/

# 3. Restore dependencies
RUN dotnet restore src/StarterKit.Api/StarterKit.Api.csproj

# 4. Copy the rest of the source code
COPY src/ src/

# 5. [CRITICAL] Remove any existing SQLite migrations from local development
# We need a clean slate to generate PostgreSQL specific migrations
RUN rm -rf src/StarterKit.Infrastructure/Data/Migrations

# 6. [CRITICAL] Generate New Migrations for PostgreSQL
# We set the DatabaseProvider to Postgres so EF Core uses Npgsql
# This ensures proper Auto Increment (Identity) generation for Postgres
ENV DatabaseProvider=Postgres
RUN dotnet ef migrations add InitialCreate \
    --project src/StarterKit.Infrastructure \
    --startup-project src/StarterKit.Api \
    --output-dir Data/Migrations

# 7. Publish the application (this compiles the new migration code)
WORKDIR /src/src/StarterKit.Api
RUN dotnet publish -c Release -o /app/publish

# Stage 2: Create the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Install basic utilities
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*
RUN mkdir -p /app/media

# Copy the published output from the build stage
COPY --from=build /app/publish .

# Copy entrypoint script
COPY entrypoint.sh .
# Ensure line endings are correct and script is executable
RUN sed -i 's/\r$//' entrypoint.sh && chmod +x entrypoint.sh

EXPOSE 5005
VOLUME ["/app/media"]
ENTRYPOINT ["./entrypoint.sh"]
# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /source

# Copy solution file
COPY *.sln .

# Copy all project files
COPY NOVA.API/*.csproj ./NOVA.API/
COPY NOVA.Application/*.csproj ./NOVA.Application/
COPY NOVA.Core/*.csproj ./NOVA.Core/
COPY NOVA.Infrastructure/*.csproj ./NOVA.Infrastructure/

# Restore dependencies
RUN dotnet restore

# Copy everything else
COPY . .

# Build and publish
WORKDIR /source/NOVA.API
RUN dotnet publish -c Release -o /app --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

# Install SQLite
RUN apt-get update && \
    apt-get install -y sqlite3 libsqlite3-dev && \
    rm -rf /var/lib/apt/lists/*

# Create data directory
RUN mkdir -p /data && chmod 777 /data

# Copy published app
COPY --from=build /app .

# Set environment
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:$PORT

EXPOSE 5000

ENTRYPOINT ["dotnet", "NOVA.API.dll"]
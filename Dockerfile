# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy project files and restore dependencies
COPY ./get-ip/get-ip.csproj ./
RUN dotnet restore

# Copy remaining files and build
COPY ./get-ip/ ./
RUN dotnet publish -c Release -o out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .

# Set the entrypoint
ENTRYPOINT ["dotnet", "get-ip.dll"]
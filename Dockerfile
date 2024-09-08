## Use the official .NET 8.0 ASP.NET runtime image as the base image
#FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
#USER app
#WORKDIR /app
#EXPOSE 8080
#
#
## Use the official .NET 8.0 SDK image as the build environment
#FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
#ARG BUILD_CONFIGURATION=Release
#WORKDIR /src
#COPY ["SpliterX-API.csproj", "."]
#RUN dotnet restore "./SpliterX-API.csproj"
#COPY . .
#WORKDIR "/src"
#RUN dotnet build "./SpliterX-API.csproj" -c $BUILD_CONFIGURATION -o /app/build
#
## Publish the application
#FROM build AS publish
#ARG BUILD_CONFIGURATION=Release
#RUN dotnet publish "./SpliterX-API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false
#
## Final stage: use the runtime image and copy the published application
#FROM base AS final
#WORKDIR /app
#COPY --from=publish /app/publish .
#ENTRYPOINT ["dotnet", "SpliterX-API.dll"]
#














# Use the official .NET 8.0 SDK image for the development environment
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS dev
WORKDIR /src

# Install dependencies, cache them for faster rebuilds
COPY ["SpliterX-API.csproj", "."]
RUN dotnet restore

# Copy the rest of the source code
COPY . .

# Expose ports for the app (8080 for HTTP and 443 for HTTPS)
EXPOSE 8080
EXPOSE 443

# Set the build configuration to Development
ENV ASPNETCORE_ENVIRONMENT=Development
ARG BUILD_CONFIGURATION=Debug

# Build the project in Debug mode
RUN dotnet build "./SpliterX-API.csproj" -c $BUILD_CONFIGURATION

# Generate and trust HTTPS development certificates
RUN dotnet dev-certs https --trust

# Enable hot reload during development with dotnet watch
ENTRYPOINT ["dotnet", "watch", "run", "--urls", "https://0.0.0.0:443;http://0.0.0.0:8080"]


# Use the official .NET SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# Copy the project files
COPY . .

# Restore the project dependencies
RUN dotnet restore

# Build the project in release mode
RUN dotnet publish -c Release -o /app

# Use the official .NET runtime image to run the application
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy the build output from the SDK image
COPY --from=build /app .

# Copy SSL certificate
COPY server.pfx /https/cert.pfx

# Set environment variables for the certificate
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/https/cert.pfx
ENV ASPNETCORE_Kestrel__Certificates__Default__Password=k55ijW0sjfgdh

# Ensure the entry point script has the correct permissions
RUN chmod +x /app

# Set the entry point to the application
ENTRYPOINT ["dotnet", "InntalerSchachfreunde.dll"]

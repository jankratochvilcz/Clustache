# Use the official .NET 9 SDK image as a build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o out

# Use the official ASP.NET Core runtime image as the runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /src/out .

# Expose port 80
EXPOSE 8080

# Set the entry point for the container
ENTRYPOINT ["dotnet", "Clustache.dll"]
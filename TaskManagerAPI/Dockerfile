# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Expose ports for app
EXPOSE 80
EXPOSE 443

# Copy everything to the working directory in the container
COPY . ./

# Restore the dependencies of app
RUN dotnet restore

# Publish the app in Release configuration
RUN dotnet publish -c Release -o out

# Build and run tests
RUN dotnet test "TaskManagerAPI.Tests/TaskManagerAPI.Tests.csproj" --logger:trx

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Copy the published files from the build stage
COPY --from=build /app/out .

# Set the environment variable to listen on port 5000
ENV ASPNETCORE_URLS=http://+:5000

# Expose port to be accesible oustide container
EXPOSE 5000

# Entry point for the application
ENTRYPOINT ["dotnet", "TaskManagerAPI.dll"]
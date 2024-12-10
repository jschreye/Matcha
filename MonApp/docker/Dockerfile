# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source
COPY ./app/Core ./Core
COPY ./app/Infrastructure ./Infrastructure
COPY ./app/Presentation ./Presentation
RUN dotnet publish ./Presentation/MyApp.csproj -c Release -o /app

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "MyApp.dll"]
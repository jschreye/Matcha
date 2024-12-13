FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copie des fichiers de solution et des fichiers projet
COPY Matcha.sln .
COPY app/Core/Core.csproj ./app/Core/
COPY app/Infrastructure/Infrastructure.csproj ./app/Infrastructure/
COPY app/Presentation/Presentation.csproj ./app/Presentation/

# Restauration des dépendances
RUN dotnet restore

# Copie du reste des fichiers source
COPY app/ ./app/

# Publication de l'application
RUN dotnet publish ./app/Presentation/Presentation.csproj -c Release -o /app

# Étape de Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "Presentation.dll"]
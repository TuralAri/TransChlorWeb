# Étape de build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY TransChlorApi.sln ./
COPY TransChlorApi/ ./TransChlorApi/
WORKDIR /src/TransChlorApi
RUN dotnet publish -c Release -o /app/out

# Étape d'exécution
FROM mcr.microsoft.com/dotnet/runtime:9.0
WORKDIR /app
COPY --from=build /app/out ./
ENTRYPOINT ["dotnet", "TransChlorApi.dll"]
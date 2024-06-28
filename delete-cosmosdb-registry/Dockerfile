#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["delete-cosmosdb-registry/delete-cosmosdb-registry.csproj", "delete-cosmosdb-registry/"]
RUN dotnet restore "delete-cosmosdb-registry/delete-cosmosdb-registry.csproj"
COPY . .
WORKDIR "/src/delete-cosmosdb-registry"
RUN dotnet build "delete-cosmosdb-registry.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "delete-cosmosdb-registry.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "delete-cosmosdb-registry.dll"]

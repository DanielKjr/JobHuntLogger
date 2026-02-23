FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app



FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["JobHuntLogger/JobHuntLogger.csproj", "JobHuntLogger/"]
RUN dotnet restore "./JobHuntLogger/JobHuntLogger.csproj"
COPY . .
WORKDIR "/src/JobHuntLogger"
RUN dotnet build "./JobHuntLogger.csproj" -c %BUILD_CONFIGURATION% -o /app/build



FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./JobHuntLogger.csproj" -c %BUILD_CONFIGURATION% -o /app/publish /p:UseAppHost=false



FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "JobHuntLogger.dll"]

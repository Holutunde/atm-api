
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release

WORKDIR /src
# Copy the .csproj file to the working directory
COPY ["ATMAPI/Src/API/API.csproj", "ATMAPI/Src/API/"]
RUN dotnet restore "ATMAPI/Src/API/API.csproj"

# Copy the rest of the files and build the application
COPY . .
WORKDIR /src/ATMAPI/Src/API
RUN dotnet build "API.csproj" -c ${BUILD_CONFIGURATION} -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "API.csproj" -c ${BUILD_CONFIGURATION} -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ATMAPI.dll"]

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia a solution e os projetos para restaurar dependências
COPY ["app-backend-monitoracao-video.sln", "./"]
COPY ["app-backend-monitoracao-video.api/app-backend-monitoracao-video.api.csproj", "app-backend-monitoracao-video.api/"]
COPY ["app-backend-monitoracao-video.domain/app-backend-monitoracao-video.domain.csproj", "app-backend-monitoracao-video.domain/"]
COPY ["app-backend-monitoracao-video.infrastructure/app-backend-monitoracao-video.infrastructure.csproj", "app-backend-monitoracao-video.infrastructure/"]

RUN dotnet restore "app-backend-monitoracao-video.sln"

# Copia o resto do código e faz o build
COPY . .
WORKDIR "/src/app-backend-monitoracao-video.api"
RUN dotnet publish "app-backend-monitoracao-video.api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "app-backend-monitoracao-video.api.dll"]

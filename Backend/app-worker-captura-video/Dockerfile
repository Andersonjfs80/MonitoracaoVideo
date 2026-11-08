FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia o csproj e restaura
COPY ["app-worker-captura-video.csproj", "./"]
RUN dotnet restore "app-worker-captura-video.csproj"

# Copia o código fonte e faz o publish
COPY . .
RUN dotnet publish "app-worker-captura-video.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Runtime (incluindo FFmpeg para captura de vídeo)
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Instalar FFmpeg no contêiner Linux para permitir a conversão RTSP
RUN apt-get update && apt-get install -y ffmpeg && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "app-worker-captura-video.dll"]

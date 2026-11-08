using app_backend_monitoracao_video.infrastructure.Context;
using app_worker_captura_video.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace app_worker_captura_video;

public class VisionAiWorker : BackgroundService
{
    private readonly ILogger<VisionAiWorker> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly int _intervaloSegundos;

    public VisionAiWorker(ILogger<VisionAiWorker> logger, IServiceScopeFactory scopeFactory, IConfiguration configuration)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _intervaloSegundos = configuration.GetValue<int>("VisionAi:IntervaloSegundos", 5);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Vision AI Worker Iniciado às: {time}. Intervalo de {intervalo}s", DateTimeOffset.Now, _intervaloSegundos);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await AnalisarCamerasAtivas(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro no loop principal do Vision AI Worker.");
            }

            await Task.Delay(TimeSpan.FromSeconds(_intervaloSegundos), stoppingToken);
        }
    }

    private async Task AnalisarCamerasAtivas(CancellationToken stoppingToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<MonitoracaoVideoDbContext>();
        var frameExtractor = scope.ServiceProvider.GetRequiredService<FrameExtractionService>();
        var aiAnalyzer = scope.ServiceProvider.GetRequiredService<IVisionAiAnalyzer>();
        var eventosApiClient = scope.ServiceProvider.GetRequiredService<EventosApiClient>();

        var camerasBanco = await dbContext.Cameras.Where(c => c.Ativa).ToListAsync(stoppingToken);

        foreach (var camera in camerasBanco)
        {
            // Extrai o frame
            var frameBytes = await frameExtractor.ExtrairFrameAsync(camera, stoppingToken);

            if (frameBytes != null && frameBytes.Length > 0)
            {
                // Analisa a imagem
                var resultadoIa = await aiAnalyzer.AnalyzeImageAsync(frameBytes);

                if (resultadoIa.Detectado)
                {
                    _logger.LogInformation("IA detectou '{Tipo}' na câmera {CameraNome}. Confiança: {Confianca:P0}", resultadoIa.Tipo, camera.Nome, resultadoIa.Confianca);
                    
                    // Envia o evento para a API Backend
                    await eventosApiClient.RegistrarAlarmeAsync(camera.Id, resultadoIa.Tipo, resultadoIa.Detalhes);
                }
            }
        }
    }
}

using app_backend_monitoracao_video.domain.Entities;
using app_backend_monitoracao_video.infrastructure.Context;
using app_worker_captura_video.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace app_worker_captura_video;

public class RtspCaptureWorker : BackgroundService
{
    private readonly ILogger<RtspCaptureWorker> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly FFmpegRecordService _ffmpegService;
    private readonly ConcurrentDictionary<Guid, CancellationTokenSource> _activeCameras = new();

    public RtspCaptureWorker(ILogger<RtspCaptureWorker> logger, IServiceScopeFactory scopeFactory, FFmpegRecordService ffmpegService)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _ffmpegService = ffmpegService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Worker de Captura RTSP Iniciado às: {time}", DateTimeOffset.Now);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessarCamerasAtivas(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro no loop principal do Worker.");
            }

            // Checa a cada 30 segundos se há novas câmeras ligadas ou desligadas no banco
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }

    private async Task ProcessarCamerasAtivas(CancellationToken stoppingToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<MonitoracaoVideoDbContext>();

        var camerasBanco = await dbContext.Cameras.Where(c => c.Ativa).ToListAsync(stoppingToken);

        // Iniciar gravação para câmeras novas
        foreach (var camera in camerasBanco)
        {
            if (!_activeCameras.ContainsKey(camera.Id))
            {
                var cts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
                _activeCameras.TryAdd(camera.Id, cts);

                _logger.LogInformation("Nova câmera detectada ativa: {Nome}. Iniciando thread de captura.", camera.Nome);

                _ = Task.Run(async () =>
                {
                    try
                    {
                        await _ffmpegService.IniciarGravacaoAsync(camera, cts.Token);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Thread de gravação falhou para a câmera {Nome}", camera.Nome);
                    }
                    finally
                    {
                        _activeCameras.TryRemove(camera.Id, out _);
                    }
                }, cts.Token);
            }
        }

        // Parar gravação de câmeras que foram desativadas no banco
        var idsAtivosNoBanco = camerasBanco.Select(c => c.Id).ToHashSet();
        foreach (var activeCameraId in _activeCameras.Keys)
        {
            if (!idsAtivosNoBanco.Contains(activeCameraId))
            {
                _logger.LogWarning("Câmera ID {Id} foi desativada no banco. Cancelando gravação FFmpeg.", activeCameraId);
                if (_activeCameras.TryGetValue(activeCameraId, out var cts))
                {
                    cts.Cancel();
                }
            }
        }
    }
}

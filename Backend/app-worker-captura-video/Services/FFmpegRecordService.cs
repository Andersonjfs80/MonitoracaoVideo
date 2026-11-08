using System.Diagnostics;
using app_backend_monitoracao_video.domain.Entities;
using Microsoft.Extensions.Logging;

namespace app_worker_captura_video.Services;

public class FFmpegRecordService
{
    private readonly ILogger<FFmpegRecordService> _logger;

    public FFmpegRecordService(ILogger<FFmpegRecordService> logger)
    {
        _logger = logger;
    }

    public async Task IniciarGravacaoAsync(Camera camera, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando FFmpeg para a câmera {CameraNome} ({UrlRtsp})", camera.Nome, camera.UrlRtsp);

        string pastaDestino = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Gravacoes", camera.Id.ToString());
        Directory.CreateDirectory(pastaDestino);

        string arquivoSaida = Path.Combine(pastaDestino, "video_%Y%m%d_%H%M%S.mp4");

        // Comando FFmpeg puro para baixar o stream RTSP (TCP) e segmentar a cada 15 minutos (900 seg) sem re-encodar (copy)
        string argumentos = $"-rtsp_transport tcp -i \"{camera.UrlRtsp}\" -c copy -f segment -segment_time 900 -reset_timestamps 1 -strftime 1 \"{arquivoSaida}\"";

        var startInfo = new ProcessStartInfo
        {
            FileName = "ffmpeg",
            Arguments = argumentos,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = new Process { StartInfo = startInfo };

        try
        {
            process.Start();

            // Lendo a saída do FFmpeg (opcional, pode gerar muitos logs se não filtrar)
            _ = Task.Run(() =>
            {
                while (!process.StandardError.EndOfStream)
                {
                    var line = process.StandardError.ReadLine();
                    // Pode logar apenas erros críticos se necessário
                    // _logger.LogDebug("[FFmpeg {CameraNome}] {Line}", camera.Nome, line);
                }
            }, cancellationToken);

            await process.WaitForExitAsync(cancellationToken);
            _logger.LogWarning("FFmpeg encerrado para a câmera {CameraNome}. Código de saída: {ExitCode}", camera.Nome, process.ExitCode);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Cancelamento solicitado. Encerrando FFmpeg da câmera {CameraNome}.", camera.Nome);
            if (!process.HasExited)
            {
                process.Kill(true);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao executar FFmpeg para a câmera {CameraNome}", camera.Nome);
        }
    }
}

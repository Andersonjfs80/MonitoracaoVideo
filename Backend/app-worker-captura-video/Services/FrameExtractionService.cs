using System.Diagnostics;
using app_backend_monitoracao_video.domain.Entities;
using Microsoft.Extensions.Logging;

namespace app_worker_captura_video.Services;

public class FrameExtractionService
{
    private readonly ILogger<FrameExtractionService> _logger;

    public FrameExtractionService(ILogger<FrameExtractionService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Extrai um frame do fluxo RTSP atual da câmera e retorna os bytes da imagem JPG.
    /// </summary>
    public async Task<byte[]?> ExtrairFrameAsync(Camera camera, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Iniciando extração de frame da câmera {CameraNome} ({UrlRtsp})", camera.Nome, camera.UrlRtsp);

        string pastaDestino = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Screenshots", camera.Id.ToString());
        Directory.CreateDirectory(pastaDestino);

        string arquivoSaida = Path.Combine(pastaDestino, $"frame_{DateTime.UtcNow:yyyyMMdd_HHmmss}.jpg");

        // Comando FFmpeg para extrair apenas 1 frame (-vframes 1) ignorando áudio (-an)
        string argumentos = $"-rtsp_transport tcp -i \"{camera.UrlRtsp}\" -y -vframes 1 -q:v 2 -an \"{arquivoSaida}\"";

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

            // Esperar o processo do FFmpeg finalizar (com timeout de 5 segundos para não travar a thread)
            using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            timeoutCts.CancelAfter(TimeSpan.FromSeconds(5));

            await process.WaitForExitAsync(timeoutCts.Token);

            if (process.ExitCode == 0 && File.Exists(arquivoSaida))
            {
                var bytes = await File.ReadAllBytesAsync(arquivoSaida, cancellationToken);
                
                // Remove o arquivo após a leitura em memória para não acumular "lixo"
                File.Delete(arquivoSaida);
                
                return bytes;
            }
            else
            {
                _logger.LogWarning("Falha na extração de frame para câmera {CameraNome}. ExitCode: {ExitCode}", camera.Nome, process.ExitCode);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Timeout na extração de frame para a câmera {CameraNome}. Encerrando FFmpeg.", camera.Nome);
            if (!process.HasExited)
            {
                process.Kill(true);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao extrair frame via FFmpeg para a câmera {CameraNome}", camera.Nome);
        }

        return null;
    }
}

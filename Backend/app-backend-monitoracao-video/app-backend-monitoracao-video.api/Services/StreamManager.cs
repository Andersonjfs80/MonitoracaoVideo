using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using app_backend_monitoracao_video.domain.Types;
using Microsoft.Extensions.Logging;

namespace app_backend_monitoracao_video.api.Services;

public class StreamManager : IDisposable
{
    private readonly ILogger<StreamManager> _logger;
    private readonly string _baseHlsPath;
    
    // Armazena processos ativos: <CameraId, (Process, UltimoAcesso)>
    private readonly ConcurrentDictionary<Guid, (Process Process, DateTime LastAccess)> _activeStreams;
    private readonly Timer _cleanupTimer;

    public StreamManager(ILogger<StreamManager> logger)
    {
        _logger = logger;
        _activeStreams = new ConcurrentDictionary<Guid, (Process, DateTime)>();
        
        // Pasta temporária para HLS
        _baseHlsPath = Path.Combine(Path.GetTempPath(), "MonitoracaoVideo", "HLS");
        if (!Directory.Exists(_baseHlsPath))
            Directory.CreateDirectory(_baseHlsPath);

        // Varre a cada 10 segundos limpando processos ociosos por mais de 30s
        _cleanupTimer = new Timer(CleanupIdleStreams, null, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10));
    }

    public async Task<string> RequestStreamAsync(Guid cameraId, string rtspUrl, StreamType type = StreamType.HLS)
    {
        if (type == StreamType.WebRTC)
        {
            _logger.LogWarning("Modo WebRTC solicitado, mas ainda não suportado nativamente. Realizando fallback (mock) ou exigirá servidor externo.");
            return "webrtc://stub-not-implemented";
        }

        // Modo HLS (Default)
        var cameraHlsPath = Path.Combine(_baseHlsPath, cameraId.ToString());
        var m3u8Path = Path.Combine(cameraHlsPath, "live.m3u8");

        if (!_activeStreams.ContainsKey(cameraId))
        {
            if (!Directory.Exists(cameraHlsPath))
                Directory.CreateDirectory(cameraHlsPath);

            // Inicia o processo do FFmpeg
            var process = StartFFmpegHls(rtspUrl, cameraHlsPath);
            _activeStreams[cameraId] = (process, DateTime.UtcNow);

            // Aguarda o arquivo m3u8 ser gerado fisicamente (timeout 5s)
            var retries = 50;
            while (!File.Exists(m3u8Path) && retries > 0)
            {
                await Task.Delay(100);
                retries--;
            }
        }
        else
        {
            // Atualiza o último acesso
            var current = _activeStreams[cameraId];
            _activeStreams[cameraId] = (current.Process, DateTime.UtcNow);
        }

        return m3u8Path;
    }

    public void NotifyAccess(Guid cameraId)
    {
        if (_activeStreams.TryGetValue(cameraId, out var current))
        {
            _activeStreams[cameraId] = (current.Process, DateTime.UtcNow);
        }
    }

    private Process StartFFmpegHls(string rtspUrl, string outputDir)
    {
        var m3u8File = Path.Combine(outputDir, "live.m3u8");
        
        var arguments = $"-rtsp_transport tcp -i \"{rtspUrl}\" " +
                        $"-c:v copy -c:a aac -f hls " +
                        $"-hls_time 2 -hls_list_size 3 -hls_flags delete_segments+append_list " +
                        $"\"{m3u8File}\"";

        _logger.LogInformation($"Iniciando HLS Stream: ffmpeg {arguments}");

        var processInfo = new ProcessStartInfo
        {
            FileName = "ffmpeg",
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        var process = new Process { StartInfo = processInfo };

        process.ErrorDataReceived += (s, e) => 
        {
            if (!string.IsNullOrEmpty(e.Data))
                _logger.LogDebug($"[FFmpeg HLS] {e.Data}");
        };

        process.Start();
        process.BeginErrorReadLine();

        return process;
    }

    private void CleanupIdleStreams(object? state)
    {
        var cutoff = DateTime.UtcNow.AddSeconds(-30);

        foreach (var kvp in _activeStreams)
        {
            if (kvp.Value.LastAccess < cutoff)
            {
                _logger.LogInformation($"Encerrando stream ocioso da câmera {kvp.Key}");
                
                try
                {
                    if (!kvp.Value.Process.HasExited)
                        kvp.Value.Process.Kill();
                }
                catch { }

                _activeStreams.TryRemove(kvp.Key, out _);
                
                // Limpa a pasta HLS da câmera para não deixar lixo
                var cameraPath = Path.Combine(_baseHlsPath, kvp.Key.ToString());
                if (Directory.Exists(cameraPath))
                {
                    try { Directory.Delete(cameraPath, true); } catch { }
                }
            }
        }
    }

    public void Dispose()
    {
        _cleanupTimer?.Dispose();
        foreach (var kvp in _activeStreams)
        {
            try { if (!kvp.Value.Process.HasExited) kvp.Value.Process.Kill(); } catch { }
        }
    }
}

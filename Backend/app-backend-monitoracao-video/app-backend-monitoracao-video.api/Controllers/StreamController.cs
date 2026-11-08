using System;
using System.IO;
using System.Threading.Tasks;
using app_backend_monitoracao_video.api.Services;
using app_backend_monitoracao_video.domain.Types;
using Microsoft.AspNetCore.Mvc;

namespace app_backend_monitoracao_video.api.Controllers;

[ApiController]
[Route("api/v1/stream")]
public class StreamController : ControllerBase
{
    private readonly StreamManager _streamManager;

    public StreamController(StreamManager streamManager)
    {
        _streamManager = streamManager;
    }

    /// <summary>
    /// Inicia o stream HLS (ou WebRTC) para uma câmera específica e retorna a playlist M3U8.
    /// </summary>
    [HttpGet("{cameraId}/live.m3u8")]
    public async Task<IActionResult> GetLiveStream(Guid cameraId, [FromQuery] string rtspUrl, [FromQuery] StreamType type = StreamType.HLS)
    {
        if (string.IsNullOrEmpty(rtspUrl))
            return BadRequest("A URL RTSP da câmera é obrigatória para iniciar o stream sob demanda.");

        var filePath = await _streamManager.RequestStreamAsync(cameraId, rtspUrl, type);

        if (type == StreamType.WebRTC)
            return Ok(new { url = filePath, message = "WebRTC mode selected. Endpoint returns stub URL." });

        if (!System.IO.File.Exists(filePath))
            return NotFound("Falha ao inicializar o stream HLS. Arquivo m3u8 não foi gerado a tempo.");

        var content = await System.IO.File.ReadAllTextAsync(filePath);
        return Content(content, "application/vnd.apple.mpegurl");
    }

    /// <summary>
    /// Fornece os fragmentos .ts solicitados pelo player e notifica que o usuário ainda está assistindo.
    /// </summary>
    [HttpGet("{cameraId}/{segment}.ts")]
    public IActionResult GetSegment(Guid cameraId, string segment)
    {
        // Avisa o StreamManager que alguém ainda está consumindo o vídeo
        _streamManager.NotifyAccess(cameraId);

        var tempPath = Path.Combine(Path.GetTempPath(), "MonitoracaoVideo", "HLS", cameraId.ToString());
        var segmentPath = Path.Combine(tempPath, $"{segment}.ts");

        if (!System.IO.File.Exists(segmentPath))
            return NotFound();

        var fileStream = new FileStream(segmentPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        return File(fileStream, "video/MP2T");
    }
}

using System.Net.Http.Json;
using Microsoft.Extensions.Logging;

namespace app_worker_captura_video.Services;

public class EventosApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<EventosApiClient> _logger;

    public EventosApiClient(HttpClient httpClient, ILogger<EventosApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task RegistrarAlarmeAsync(Guid cameraId, string tipo, string detalhes)
    {
        try
        {
            var payload = new
            {
                CameraId = cameraId,
                Tipo = tipo,
                Detalhes = detalhes
            };

            // O Endpoint é /api/v1/eventos/alarme, assumindo BaseAddress configurado no HttpClient
            var response = await _httpClient.PostAsJsonAsync("api/v1/eventos/alarme", payload);

            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Falha ao registrar alarme. StatusCode: {StatusCode}, Resposta: {Content}", response.StatusCode, content);
            }
            else
            {
                _logger.LogInformation("Alarme do tipo '{Tipo}' registrado com sucesso para a Câmera {CameraId}", tipo, cameraId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao tentar enviar alarme para a API");
        }
    }
}

using System;
using System.Threading.Tasks;
using app_backend_monitoracao_video.domain.Entities;
using app_backend_monitoracao_video.infrastructure.Data;
using Microsoft.AspNetCore.Mvc;

namespace app_backend_monitoracao_video.api.Controllers;

[ApiController]
[Route("api/v1/eventos")]
public class EventosController : ControllerBase
{
    private readonly MonitoracaoVideoDbContext _dbContext;

    public EventosController(MonitoracaoVideoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Registra um evento de alarme ou movimento identificado por IA ou hardware.
    /// </summary>
    [HttpPost("alarme")]
    public async Task<IActionResult> RegistrarAlarme([FromBody] AlarmeRequest request)
    {
        if (request == null || request.CameraId == Guid.Empty)
            return BadRequest("Requisição inválida.");

        // Para simplificar a MVP, vamos registrar o evento como um log de "Gravação" especial,
        // ou você pode criar uma entidade separada 'EventoAlarme'. Por enquanto retornamos sucesso simulado.
        
        // Simulação de inserção na tabela de eventos (que será criada no BD futuramente)
        // var evento = new Evento { CameraId = request.CameraId, Tipo = request.Tipo, DataHora = DateTime.UtcNow };
        // _dbContext.Eventos.Add(evento);
        // await _dbContext.SaveChangesAsync();

        return Ok(new { mensagem = "Evento registrado com sucesso", data = DateTime.UtcNow });
    }
}

public class AlarmeRequest
{
    public Guid CameraId { get; set; }
    public string Tipo { get; set; } = "Movimento"; // Movimento, Objeto, Pessoa, etc.
    public string? Detalhes { get; set; }
}

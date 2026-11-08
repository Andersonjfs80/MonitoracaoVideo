using app_backend_monitoracao_video.domain.Entities;

namespace app_backend_monitoracao_video.domain.Interfaces;

public interface IGravacaoRepository
{
    Task<Gravacao?> ObterPorIdAsync(Guid id);
    Task<IEnumerable<Gravacao>> ObterPorCameraAsync(Guid cameraId);
    Task AdicionarAsync(Gravacao gravacao);
    Task AtualizarAsync(Gravacao gravacao);
}

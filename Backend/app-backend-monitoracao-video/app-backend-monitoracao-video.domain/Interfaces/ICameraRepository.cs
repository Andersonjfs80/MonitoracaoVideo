using app_backend_monitoracao_video.domain.Entities;

namespace app_backend_monitoracao_video.domain.Interfaces;

public interface ICameraRepository
{
    Task<Camera?> ObterPorIdAsync(Guid id);
    Task<IEnumerable<Camera>> ObterTodasAsync();
    Task AdicionarAsync(Camera camera);
    Task AtualizarAsync(Camera camera);
    Task RemoverAsync(Guid id);
}

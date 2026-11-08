using app_backend_monitoracao_video.domain.Entities;

namespace app_backend_monitoracao_video.domain.Interfaces;

public interface IReconhecimentoPlacaRepository
{
    Task<IEnumerable<ReconhecimentoPlaca>> ObterPorCameraAsync(Guid cameraId);
    Task<IEnumerable<ReconhecimentoPlaca>> ObterPorPlacaAsync(string placa);
    Task AdicionarAsync(ReconhecimentoPlaca reconhecimento);
}

namespace app_backend_monitoracao_video.domain.Entities;

public class ReconhecimentoPlaca
{
    public Guid Id { get; private set; }
    public Guid CameraId { get; private set; }
    public Camera Camera { get; private set; }
    public string Placa { get; private set; }
    public decimal Confianca { get; private set; }
    public string CaminhoSnapshot { get; private set; }
    public DateTime DataCaptura { get; private set; }

    protected ReconhecimentoPlaca() { }

    public ReconhecimentoPlaca(Guid cameraId, string placa, decimal confianca, string caminhoSnapshot)
    {
        Id = Guid.NewGuid();
        CameraId = cameraId;
        Placa = placa;
        Confianca = confianca;
        CaminhoSnapshot = caminhoSnapshot;
        DataCaptura = DateTime.UtcNow;
    }
}

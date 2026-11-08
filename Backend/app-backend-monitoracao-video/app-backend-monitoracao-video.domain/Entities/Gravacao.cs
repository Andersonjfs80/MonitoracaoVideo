namespace app_backend_monitoracao_video.domain.Entities;

public enum TipoEventoGravacao
{
    Continua = 1,
    Movimento = 2,
    Alarme = 3,
    ReconhecimentoPlaca = 4
}

public class Gravacao
{
    public Guid Id { get; private set; }
    public Guid CameraId { get; private set; }
    public Camera Camera { get; private set; }
    public string CaminhoArquivo { get; private set; }
    public DateTime DataInicio { get; private set; }
    public DateTime? DataFim { get; private set; }
    public long TamanhoBytes { get; private set; }
    public TipoEventoGravacao TipoEvento { get; private set; }

    protected Gravacao() { }

    public Gravacao(Guid cameraId, string caminhoArquivo, DateTime dataInicio, TipoEventoGravacao tipoEvento)
    {
        Id = Guid.NewGuid();
        CameraId = cameraId;
        CaminhoArquivo = caminhoArquivo;
        DataInicio = dataInicio;
        TipoEvento = tipoEvento;
        TamanhoBytes = 0;
    }

    public void FinalizarGravacao(DateTime dataFim, long tamanhoBytes)
    {
        DataFim = dataFim;
        TamanhoBytes = tamanhoBytes;
    }
}

using System.Threading.Tasks;

namespace app_worker_captura_video.Services;

public class VisionAiResult
{
    public bool Detectado { get; set; }
    public string Tipo { get; set; } = string.Empty; // "Movimento", "Pessoa", "Placa"
    public string Detalhes { get; set; } = string.Empty;
    public double Confianca { get; set; }
}

public interface IVisionAiAnalyzer
{
    Task<VisionAiResult> AnalyzeImageAsync(byte[] imageBytes);
}

public class DummyVisionAiAnalyzer : IVisionAiAnalyzer
{
    private readonly Random _random = new Random();

    public Task<VisionAiResult> AnalyzeImageAsync(byte[] imageBytes)
    {
        // Mock: Simula processamento da IA
        // Retorna positivo com 10% de probabilidade
        bool detectado = _random.Next(0, 10) == 0; 
        
        string[] tipos = { "Movimento", "Pessoa", "Veículo" };
        string tipo = tipos[_random.Next(tipos.Length)];

        var result = new VisionAiResult
        {
            Detectado = detectado,
            Tipo = tipo,
            Detalhes = detectado ? $"Simulação: {tipo} detectado com confiança." : string.Empty,
            Confianca = detectado ? _random.Next(80, 100) / 100.0 : 0.0
        };

        return Task.FromResult(result);
    }
}

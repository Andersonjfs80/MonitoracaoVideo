namespace app_backend_monitoracao_video.domain.Entities;

public class Camera
{
    public Guid Id { get; private set; }
    public string Nome { get; private set; }
    public string UrlRtsp { get; private set; }
    public string Usuario { get; private set; }
    public string Senha { get; private set; }
    public string Fabricante { get; private set; }
    public bool Ativa { get; private set; }
    public string IpEndereco { get; private set; }
    public DateTime DataCriacao { get; private set; }

    protected Camera() { } // Para o EF Core

    public Camera(string nome, string urlRtsp, string usuario, string senha, string fabricante, string ipEndereco)
    {
        Id = Guid.NewGuid();
        Nome = nome;
        UrlRtsp = urlRtsp;
        Usuario = usuario;
        Senha = senha;
        Fabricante = fabricante;
        IpEndereco = ipEndereco;
        Ativa = true;
        DataCriacao = DateTime.UtcNow;
    }

    public void AtualizarStatus(bool ativa)
    {
        Ativa = ativa;
    }

    public void AtualizarCredenciais(string usuario, string senha)
    {
        Usuario = usuario;
        Senha = senha;
    }
}

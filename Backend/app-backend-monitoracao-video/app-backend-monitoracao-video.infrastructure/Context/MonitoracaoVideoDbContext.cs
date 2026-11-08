using app_backend_monitoracao_video.domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace app_backend_monitoracao_video.infrastructure.Context;

public class MonitoracaoVideoDbContext : DbContext
{
    public MonitoracaoVideoDbContext(DbContextOptions<MonitoracaoVideoDbContext> options) : base(options) { }

    public DbSet<Camera> Cameras { get; set; }
    public DbSet<Gravacao> Gravacoes { get; set; }
    public DbSet<ReconhecimentoPlaca> ReconhecimentosPlaca { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}

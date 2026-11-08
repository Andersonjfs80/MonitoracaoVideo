using app_backend_monitoracao_video.domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace app_backend_monitoracao_video.infrastructure.Mappings;

public class GravacaoMapping : IEntityTypeConfiguration<Gravacao>
{
    public void Configure(EntityTypeBuilder<Gravacao> builder)
    {
        builder.HasKey(g => g.Id);

        builder.Property(g => g.CaminhoArquivo)
            .IsRequired()
            .HasColumnType("varchar(1000)");

        // Configuração do Enum
        builder.Property(g => g.TipoEvento)
            .IsRequired()
            .HasConversion<int>();

        // Relacionamento com Camera
        builder.HasOne(g => g.Camera)
            .WithMany()
            .HasForeignKey(g => g.CameraId);

        builder.ToTable("Gravacoes");
    }
}

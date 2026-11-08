using app_backend_monitoracao_video.domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace app_backend_monitoracao_video.infrastructure.Mappings;

public class ReconhecimentoPlacaMapping : IEntityTypeConfiguration<ReconhecimentoPlaca>
{
    public void Configure(EntityTypeBuilder<ReconhecimentoPlaca> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Placa)
            .IsRequired()
            .HasColumnType("varchar(20)");

        builder.Property(r => r.Confianca)
            .HasColumnType("decimal(5,2)");

        builder.Property(r => r.CaminhoSnapshot)
            .IsRequired()
            .HasColumnType("varchar(1000)");

        // Relacionamento com Camera
        builder.HasOne(r => r.Camera)
            .WithMany()
            .HasForeignKey(r => r.CameraId);

        builder.ToTable("ReconhecimentosPlaca");
    }
}

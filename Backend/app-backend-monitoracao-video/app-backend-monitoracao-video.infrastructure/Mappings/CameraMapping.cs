using app_backend_monitoracao_video.domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace app_backend_monitoracao_video.infrastructure.Mappings;

public class CameraMapping : IEntityTypeConfiguration<Camera>
{
    public void Configure(EntityTypeBuilder<Camera> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Nome)
            .IsRequired()
            .HasColumnType("varchar(150)");

        builder.Property(c => c.UrlRtsp)
            .IsRequired()
            .HasColumnType("varchar(500)");

        builder.Property(c => c.Usuario)
            .HasColumnType("varchar(100)");

        builder.Property(c => c.Senha)
            .HasColumnType("varchar(255)");

        builder.Property(c => c.Fabricante)
            .HasColumnType("varchar(100)");

        builder.Property(c => c.IpEndereco)
            .HasColumnType("varchar(50)");

        builder.ToTable("Cameras");
    }
}

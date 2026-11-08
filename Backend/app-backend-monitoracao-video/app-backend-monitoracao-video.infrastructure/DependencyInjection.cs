using app_backend_monitoracao_video.domain.Interfaces;
using app_backend_monitoracao_video.infrastructure.Context;
//using app_backend_monitoracao_video.infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace app_backend_monitoracao_video.infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<MonitoracaoVideoDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(MonitoracaoVideoDbContext).Assembly.FullName)));

        // Repositórios a serem injetados aqui posteriormente
        // services.AddScoped<ICameraRepository, CameraRepository>();
        // services.AddScoped<IGravacaoRepository, GravacaoRepository>();
        // services.AddScoped<IReconhecimentoPlacaRepository, ReconhecimentoPlacaRepository>();

        return services;
    }
}

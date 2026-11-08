using app_backend_monitoracao_video.infrastructure;
using app_worker_captura_video;
using app_worker_captura_video.Services;
using Core_Logs.IoC;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;

try
{
    var builder = Host.CreateApplicationBuilder(args);

    // Injeção do Core_Logs (Log centralizado, Kafka, Elasticsearch)
    builder.Services.AddCoreLogs(builder.Configuration);

    // Injeção da Infraestrutura (DbContext)
    builder.Services.AddInfrastructure(builder.Configuration);

    // Registra os serviços do Worker
    builder.Services.AddSingleton<FFmpegRecordService>();
    builder.Services.AddHostedService<RtspCaptureWorker>();

    // Registra os novos serviços da Fase 4 (Visão Computacional)
    builder.Services.AddSingleton<FrameExtractionService>();
    builder.Services.AddSingleton<IVisionAiAnalyzer, DummyVisionAiAnalyzer>();
    
    // Configura o HttpClient do EventosApiClient lendo a URL do appsettings
    builder.Services.AddHttpClient<EventosApiClient>(client =>
    {
        var baseUrl = builder.Configuration.GetValue<string>("ApiBackend:BaseUrl");
        client.BaseAddress = new Uri(baseUrl ?? "http://localhost:5000");
    });

    builder.Services.AddHostedService<VisionAiWorker>();

    var host = builder.Build();
    host.Run();
}
catch (Exception ex)
{
    Console.WriteLine($"Falha fatal ao iniciar o Worker de Captura de Video: {ex.Message}");
}

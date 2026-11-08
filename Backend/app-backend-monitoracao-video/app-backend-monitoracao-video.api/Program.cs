using app_backend_monitoracao_video.infrastructure;
using app_backend_monitoracao_video.api.Services;
using Core_Logs.IoC;
using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Substituindo Serilog raiz pelo nosso Core_Logs corporativo
builder.Services.AddCoreLogs(builder.Configuration);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Injeção do Gerenciador de Stream Dinâmico
builder.Services.AddSingleton<StreamManager>();

// Injeção da Infraestrutura
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Interceptadores do Core_Logs
app.UseGlobalErrorHandler();
app.UseKafkaLogging();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

using Microsoft.Extensions.Hosting;
using notification.Services;
using Npgsql;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);
var microserviceName = builder.Configuration["MicroserviceName"] ?? "Notification.Api";
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<EmailService>();
builder.Services.AddHostedService<KafkaConsumerService>();
// Define a common resource to correlate data in Grafana/Jaeger
var resourceBuilder = ResourceBuilder.CreateDefault()
    .AddService(microserviceName ?? "Notification.Api");

var opentelemetry = builder.Services.AddOpenTelemetry();

opentelemetry.ConfigureResource(resource => resource.AddService(microserviceName ?? "Notification.Api"))
.WithTracing(tracing =>
{
    tracing
        .AddSource(microserviceName ?? "Notification.Api") // Add the source for manual spans
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddNpgsql()
        .AddSource("KafkaConsumer");

    tracing.AddOtlpExporter(option =>
    {
        option.Endpoint = new Uri(builder.Configuration["Otlp:Endpoint"]!);
        option.Protocol = OtlpExportProtocol.Grpc;
    });
});

opentelemetry.WithMetrics(metrics => metrics
    .AddAspNetCoreInstrumentation()
    .AddMeter("Microsoft.AspNetCore.Hosting")
    .AddMeter("Microsoft.AspNetCore.Server.Kestrel")
    .AddMeter("System.Net.Http")
    .AddMeter("System.Net.NameResolution")
    .AddOtlpExporter(option =>
    {
        option.Endpoint = new Uri(builder.Configuration["Otlp:Endpoint"]!);
        option.Protocol = OtlpExportProtocol.Grpc;
    }));

opentelemetry.WithLogging(logging =>
{
    //logging.SetResourceBuilder(resourceBuilder);
    logging.AddOtlpExporter(option =>
    {
        option.Endpoint = new Uri(builder.Configuration["Otlp:Endpoint"]!);
        option.Protocol = OtlpExportProtocol.Grpc;
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //builder.Configuration.AddUserSecrets<Program>();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

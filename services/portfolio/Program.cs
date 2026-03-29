using Amazon;
using Amazon.SimpleNotificationService;
using api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Npgsql;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using portfolio.Data;
using portfolio.Services;

var builder = WebApplication.CreateBuilder(args);
var environment = builder.Environment.EnvironmentName;
var microserviceName = builder.Configuration["MicroserviceName"];
if (!builder.Environment.IsDevelopment())
{
    //builder.Configuration.AddSecretsManager(region: RegionEndpoint.USEast1, configurator: config => {
    //    config.SecretFilter = record => record.Name.StartsWith($"{environment}_{microserviceName}_");
    //    config.KeyGenerator = (_, name) => name
    //                    .Replace($"{environment}_{microserviceName}_", string.Empty)
    //                    .Replace("__", ":");
    //});
}
else
{
    //builder.Configuration.AddUserSecrets<Program>();
}
builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Smart Invest", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme =
    options.DefaultChallengeScheme =
    options.DefaultForbidScheme =
    options.DefaultScheme =
    options.DefaultSignInScheme =
    options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:Audience"],
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]!)
        )
    };
});

builder.Services.AddDbContext<ApplicationDBContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DbConnectionString")));
builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
builder.Services.AddAWSService<IAmazonSimpleNotificationService>();
builder.Services.AddHttpClient<MarketDataService>();
builder.Services.AddScoped<StockService>();
builder.Services.AddScoped<IMarketDataService, MarketDataService>();
builder.Services.AddScoped<IStockService, StockService>();
builder.Services.AddScoped<IPortfolioService, PortfolioService>();
builder.Services.AddScoped<IAnalysisService, AnalysisService>();
builder.Services.AddHttpClient<AnalysisService>(client =>
{
    client.DefaultRequestHeaders.Add("Authorization", $"Bearer ");
});
builder.Services.AddSingleton<KafkaClientHandle>();
builder.Services.AddSingleton<KafkaProducerService<string, string>>();

// Define a common resource to correlate data in Grafana/Jaeger
var resourceBuilder = ResourceBuilder.CreateDefault()
    .AddService(microserviceName ?? "Portfolio.Api");

var opentelemetry = builder.Services.AddOpenTelemetry();

opentelemetry.ConfigureResource(resource => resource.AddService(microserviceName ?? "Portfolio.Api"))
.WithTracing(tracing =>
{
    tracing
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddNpgsql();

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
        var test = builder.Configuration["Otlp:Endpoint"];
        option.Endpoint = new Uri(builder.Configuration["Otlp:Endpoint"]!);
        option.Protocol = OtlpExportProtocol.Grpc;
    });
});

var app = builder.Build();


using (var Scope = app.Services.CreateScope())
{
    var context = Scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
    context.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(x => x
     .AllowAnyMethod()
     .AllowAnyHeader()
     .AllowCredentials()
      .WithOrigins("https://localhost:5173")
      .SetIsOriginAllowed(origin => true));

app.UseAuthentication();
app.UseAuthorization();

app.UseAuthorization();

app.MapControllers();

app.Run();

// configure services and DI
using api;
using api.Data;
using api.Models;
using api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Amazon;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);
var environment = builder.Environment.EnvironmentName;
var microserviceName = builder.Configuration["MicroserviceName"];
builder.Configuration.AddSecretsManager(region: RegionEndpoint.USEast1, configurator: config => {
    config.SecretFilter = record => record.Name.StartsWith($"{environment}_{microserviceName}_");
    config.KeyGenerator = (_, name) => name
                    .Replace($"{environment}_{microserviceName}_", string.Empty)
                    .Replace("__", ":");
});
builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
builder.Services.AddControllers();
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

builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
});

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
    });
});

opentelemetry.WithMetrics(metrics => metrics
    .AddAspNetCoreInstrumentation()
    .AddMeter("Microsoft.AspNetCore.Hosting")
    .AddMeter("Microsoft.AspNetCore.Server.Kestrel")
    .AddMeter("System.Net.Http")
    .AddMeter("System.Net.NameResolution")
    .AddPrometheusExporter(option =>
    {
        option.ScrapeEndpointPath = "/metrics";
    }));

builder.Services.AddDbContext<ApplicationDBContext>(options => options.UseMySql(builder.Configuration.GetConnectionString("DbConnectionString"), new MySqlServerVersion(new Version(8, 0, 35))));
builder.Services.AddSingleton<KafkaClientHandle>();
builder.Services.AddSingleton<KafkaProducerService<string, string>>();

builder.Services.AddScoped<TokenService>();
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 12;
})
.AddEntityFrameworkStores<ApplicationDBContext>();

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


// control http request pipeline
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

app.MapControllers();

app.UseHttpsRedirection();
app.UseCors(x => x
     .AllowAnyMethod()
     .AllowAnyHeader()
     .AllowCredentials()
      .WithOrigins("https://localhost:5173")
      .SetIsOriginAllowed(origin => true));

app.UseAuthentication();
app.UseAuthorization();

app.Run();
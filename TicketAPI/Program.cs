using MediatR;
using TicketAPI.Data;
using Microsoft.EntityFrameworkCore;
using Prometheus;
using Serilog;
using StackExchange.Redis;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Microsoft.ApplicationInsights.Extensibility;
using Serilog.Sinks.ApplicationInsights.TelemetryConverters;
using TicketAPI.Middleware;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", Serilog.Events.LogEventLevel.Debug)
    .WriteTo.Console()
    .CreateLogger();

try
{
    Log.Information("Uygulama baslatiliyor...");

    var builder = WebApplication.CreateBuilder(args);

    var applicationInsightsConnectionString = builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"];

    Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Information()
        .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", Serilog.Events.LogEventLevel.Debug)
        .Enrich.FromLogContext()
        .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
        .WriteTo.ApplicationInsights(
            new TelemetryConfiguration
            {
                ConnectionString = applicationInsightsConnectionString
            },
            TelemetryConverter.Traces)
        .CreateLogger();

    builder.Host.UseSerilog();

    var connectionString = builder.Configuration.GetConnectionString("Supabase");

    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(connectionString));

    var redisConnectionString = builder.Configuration.GetConnectionString("Redis");

    var configurationOptions = ConfigurationOptions.Parse(redisConnectionString!);
    // Yerel bilgisayardaki Firewall/SSL engellerini asmak icin kritik ayar:
    configurationOptions.CertificateValidation += delegate { return true; };

    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.ConfigurationOptions = configurationOptions;
        options.InstanceName = "TicketAPI_";
    });

    builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
        ConnectionMultiplexer.Connect(configurationOptions));

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
        .AddJwtBearer(options =>
        {
            var issuer = builder.Configuration["SupabaseAuth:Issuer"]!;
            var audience = builder.Configuration["SupabaseAuth:Audience"]!;

            options.Authority = issuer;

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidateAudience = true,
                ValidAudience = audience,
                ValidateLifetime = true
            };
        });

    builder.Services.AddMediatR(typeof(Program));
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowFrontend", policy =>
            policy.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
    });
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "TicketAPI", Version = "v1" });

        // Swagger'da JWT Bearer Authorization butonunu ekleyelim
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme. Ornek: \"Bearer {token}\"",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });

        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[] {}
            }
        });
    });

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseMiddleware<ExceptionHandlingMiddleware>();

    app.UseHttpsRedirection();

    app.UseCors("AllowFrontend");

    app.UseAuthentication();
    app.UseAuthorization();

    app.UseHttpMetrics();

    app.MapControllers();

    app.MapMetrics();

    app.MapGet("/", () => "TicketAPI running in Italy server.");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Uygulama beklenmedik bir sekilde coktu!");
}
finally
{
    Log.CloseAndFlush();
}

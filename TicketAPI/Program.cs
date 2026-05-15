using MediatR;
using TicketAPI.Data;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Prometheus;
using Serilog;
using StackExchange.Redis;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

try
{
    Log.Information("Uygulama baslatiliyor...");

    var builder = WebApplication.CreateBuilder(args);

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

    builder.Services.AddMediatR(typeof(Program));

    var jwtSecret = builder.Configuration["SupabaseAuth:JwtSecret"];
    var jwtIssuer = builder.Configuration["SupabaseAuth:Issuer"];
    var jwtAudience = builder.Configuration["SupabaseAuth:Audience"];

    if (string.IsNullOrWhiteSpace(jwtSecret) ||
        string.IsNullOrWhiteSpace(jwtIssuer) ||
        string.IsNullOrWhiteSpace(jwtAudience))
    {
        throw new InvalidOperationException(
            "SupabaseAuth ayarlari eksik. SupabaseAuth:JwtSecret, SupabaseAuth:Issuer ve SupabaseAuth:Audience tanimlanmali.");
    }

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
                ValidateIssuer = true,
                ValidIssuer = jwtIssuer,
                ValidateAudience = true,
                ValidAudience = jwtAudience,
                ValidateLifetime = true,
                NameClaimType = "sub",
                RoleClaimType = ClaimTypes.Role
            };
        });

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowFrontend", policy =>
            policy.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
    });
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

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

using MediatR;
using TicketAPI.Data;
using Microsoft.EntityFrameworkCore;
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

    app.UseHttpMetrics();

    app.MapControllers();

    app.MapMetrics();

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

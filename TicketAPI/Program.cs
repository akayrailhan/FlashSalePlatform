using TicketAPI.Data;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;


var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Supabase");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

var redisConnectionString = builder.Configuration.GetConnectionString("Redis");

var configurationOptions = ConfigurationOptions.Parse(redisConnectionString!);
// Yerel bilgisayardaki Firewall/SSL engellerini aşmak için kritik ayar:
configurationOptions.CertificateValidation += delegate { return true; };

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.ConfigurationOptions = configurationOptions;
    options.InstanceName = "TicketAPI_";
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

app.MapControllers();

app.Run();

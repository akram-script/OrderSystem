
using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using Shared.Messages;

var builder = WebApplication.CreateBuilder(args);

// ── Services ──────────────────────────────────────────────────────
builder.Services.AddControllers();  
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<RabbitMqPublisher>();

// ── Build ─────────────────────────────────────────────────────────
var app = builder.Build();

await app.Services.GetRequiredService<RabbitMqPublisher>().InitializeAsync();

// ── Middleware ────────────────────────────────────────────────────
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();  

app.Run();

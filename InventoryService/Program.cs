using InventoryService.Workers;
using Shared.Messages;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddSingleton<RabbitMqPublisher>();
builder.Services.AddHostedService<InventoryWorker>();

var host = builder.Build();

// Initialize the publisher before the host starts
var publisher = host.Services.GetRequiredService<RabbitMqPublisher>();
await publisher.InitializeAsync();

await host.RunAsync();
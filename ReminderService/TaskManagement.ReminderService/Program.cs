using Microsoft.EntityFrameworkCore;
using Serilog;
using TaskManagement.ReminderService;
using TaskManagement.ReminderService.Configuration;
using TaskManagement.ReminderService.Data;
using TaskManagement.ReminderService.Services;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.File(
        path: "logs/reminder-service-.log",
        rollingInterval: RollingInterval.Day,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

try
{
    Log.Information("Starting Task Reminder Service...");

    var builder = Host.CreateApplicationBuilder(args);

    builder.Services.Configure<RabbitMQSettings>(
        builder.Configuration.GetSection(RabbitMQSettings.SectionName));

    builder.Services.AddSerilog();

    builder.Services.AddDbContext<ReminderDbContext>(options =>
    {
        options.UseSqlServer(
            builder.Configuration.GetConnectionString("DefaultConnection"),
            sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorNumbersToAdd: null);
            });
    });

    builder.Services.AddSingleton<IRabbitMQService, RabbitMQService>();
    builder.Services.AddSingleton<ITaskReminderService, TaskReminderService>();
    builder.Services.AddHostedService<ReminderWorker>();

    builder.Services.AddWindowsService(options =>
    {
        options.ServiceName = "Task Reminder Service";
    });

    var host = builder.Build();

    Log.Information("Service configured successfully. Starting host...");

    await host.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Service terminated unexpectedly");
    throw;
}
finally
{
    Log.Information("Service shutdown complete");
    await Log.CloseAndFlushAsync();
}

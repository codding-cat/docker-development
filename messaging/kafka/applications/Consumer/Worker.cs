using System.Text;
using Kafka.Public;
using Kafka.Public.Loggers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Consumer;

public class Worker : IHostedService
{
    private readonly ILogger<Worker> _logger;
    private readonly IHostApplicationLifetime _appLifetime;
    private readonly ClusterClient _cluster;
    private readonly IConfiguration _configuration;

    public Worker(ILogger<Worker> logger, IHostApplicationLifetime appLifetime, IConfiguration configuration)
    {
        _logger = logger;
        _appLifetime = appLifetime;
        _cluster = new ClusterClient(new Configuration
        {
            Seeds = "kafka-1:9092,kafka-2:9092,kafka-3:9092"
        }, new ConsoleLogger());
        _configuration = configuration;
    }
    
    public Task StartAsync(CancellationToken cancellationToken)
    {
       // Application lifetime methods should be done this way!
        _appLifetime.ApplicationStarted.Register(OnStarted);
        _appLifetime.ApplicationStopping.Register(OnStopping);
        _appLifetime.ApplicationStopped.Register(OnStopped);
        
        _logger.LogInformation("StartAsync has been called");
        
        _cluster.ConsumeFromEarliest("Orders");
        _cluster.MessageReceived += record =>
        {
            var stringReceived = Encoding.UTF8.GetString(record.Value as byte[] ?? Array.Empty<byte>());
            _logger.LogInformation("Record received: {Record}", stringReceived);
        };

        return Task.CompletedTask;
    }
    
    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("StopAsync has been called");
        return Task.CompletedTask;
    }

    private void OnStopped()
    {
        _logger.LogInformation("Application have stopped");
    }

    private void OnStopping()
    {
        _cluster?.Dispose();
        _logger.LogInformation("Application stopping");
    }

    private void OnStarted()
    {
        _logger.LogInformation("Application started");
    }
    
}
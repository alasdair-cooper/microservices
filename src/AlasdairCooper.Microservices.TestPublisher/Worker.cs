using AlasdairCooper.Microservices.Shared;

using MassTransit;

namespace AlasdairCooper.Microservices.TestPublisher;

public class Worker(ILogger<Worker> logger, IServiceProvider serviceProvider) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var endpoint = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<IPublishEndpoint>();
        
        while (!stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation("Worker running at: {Time}", DateTimeOffset.Now);

            await endpoint.Publish(new Messages.TestMessage(DateTime.Now), stoppingToken);

            await Task.Delay(1000, stoppingToken);
        }
    }
}
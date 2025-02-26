using System.Net.NetworkInformation;

using AlasdairCooper.Microservices.Shared;

using MassTransit;

namespace AlasdairCooper.Microservices.NetTracker;

public class Worker(IPublishEndpoint publishEndpoint, ILogger<Worker> logger) : ScopedBackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var pingSender = new Ping();

        while (!stoppingToken.IsCancellationRequested)
        {
            await PingAddress("8.8.8.8");
            await Task.Delay(250, stoppingToken);
        }

        return;

        async Task PingAddress(string address)
        {
            var info = await pingSender.SendPingAsync(address, TimeSpan.FromSeconds(5), cancellationToken: stoppingToken);
            await publishEndpoint.Publish(new Messaging.NetworkPingMessage(info.RoundtripTime), stoppingToken);
            logger.LogInformation("Pinged {Address} in {RoundtripTime} ms", address, info.RoundtripTime);
        }
    }
}
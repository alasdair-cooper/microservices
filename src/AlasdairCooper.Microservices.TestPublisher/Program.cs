using AlasdairCooper.Microservices.TestPublisher;

using MassTransit;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });
        
        cfg.ConfigureEndpoints(ctx);
    });
});

builder.Services.AddHostedService<Worker>();

var host = builder.Build();

host.Run();
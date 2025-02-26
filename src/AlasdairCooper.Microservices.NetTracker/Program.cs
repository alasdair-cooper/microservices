using System.Text.Json.Serialization;

using AlasdairCooper.Microservices.NetTracker;
using AlasdairCooper.Microservices.Shared;

using MassTransit;

using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddMassTransit(
    x =>
    {
        x.UsingRabbitMq(
            (ctx, cfg) =>
            {
                cfg.ConfigureJsonSerializerOptions(
                    x =>
                    {
                        x.Converters.Add(new JsonFSharpConverter());
                        return x;
                    });

                var opts = ctx.GetRequiredService<IOptions<Messaging.MessagingOptions>>();

                cfg.Host(
                    opts.Value.Host,
                    opts.Value.VirtualHost,
                    h =>
                    {
                        h.Username(opts.Value.Username);
                        h.Password(opts.Value.Password);
                    });

                cfg.ConfigureEndpoints(ctx);
            });
    });

builder.Services.AddOptions<Messaging.MessagingOptions>().BindConfiguration("Messaging").ValidateDataAnnotations().ValidateOnStart();

builder.Services.AddScoped<ScopedBackgroundService, Worker>();
builder.Services.AddHostedService<ScopedBackgroundServiceProcessor>();

var host = builder.Build();
host.Run();
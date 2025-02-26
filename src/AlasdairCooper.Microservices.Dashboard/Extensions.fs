namespace AlasdairCooper.Microservices.Dashboard

open System.Runtime.CompilerServices
open System.Text.Json.Serialization
open System.Threading.Tasks
open AlasdairCooper.Microservices.Shared
open Avalonia.Threading
open MassTransit
open MassTransit.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Options

type DispatchProvider<'msg> = { Dispatch: 'msg -> unit }

type MessageConsumer<'dto when 'dto: not struct>(dispatch: 'dto -> unit) =
    interface IConsumer<'dto> with
        member this.Consume(context: ConsumeContext<_>) : Task =
            Dispatcher.UIThread.Invoke(fun _ -> dispatch context.Message)
            Task.CompletedTask

type ServiceCollectionExtensions =

    [<Extension>]
    static member inline AddDispatchProvider(services: IServiceCollection, dispatch) : IServiceCollection =
        services.AddScoped<DispatchProvider<_>>(fun _ -> { Dispatch = dispatch })

    [<Extension>]
    static member inline AddMessageConsumer(services: IServiceCollection, map: 'dto -> 'msg) : IServiceCollection =
        services
            .AddScoped<MessageConsumer<'dto>>(fun x -> MessageConsumer(map >> x.GetRequiredService<DispatchProvider<'msg>>().Dispatch))
            .RegisterConsumer<MessageConsumer<'dto>>()
        |> ignore

        services

    [<Extension>]
    static member inline AddMassTransit(services: IServiceCollection) : IServiceCollection =
        services
            .AddMassTransit(fun (x: IBusRegistrationConfigurator) ->
                x.UsingRabbitMq(fun (ctx: IBusRegistrationContext) (cfg: IRabbitMqBusFactoryConfigurator) ->
                    let opts = ctx.GetRequiredService<IOptions<MessagingOptions>>()

                    cfg.Host(
                        opts.Value.Host,
                        opts.Value.VirtualHost,
                        fun h ->
                            h.Username(opts.Value.Username)
                            h.Password(opts.Value.Password)
                    )

                    cfg.ConfigureJsonSerializerOptions(fun x ->
                        x.Converters.Add(JsonFSharpConverter())
                        x)

                    cfg.ConfigureEndpoints(ctx)))
            .AddMassTransitTextWriterLogger()
        |> ignore

        services
            .AddOptions<MessagingOptions>()
            .BindConfiguration("Messaging")
            .ValidateDataAnnotations()
            .ValidateOnStart()
        |> ignore

        services

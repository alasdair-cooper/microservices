namespace AlasdairCooper.Microservices.Dashboard

open System.Runtime.CompilerServices
open System.Threading.Tasks
open Avalonia.Threading
open MassTransit
open Microsoft.Extensions.DependencyInjection

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
            .AddScoped<MessageConsumer<_>>(fun x -> MessageConsumer(map >> x.GetRequiredService<DispatchProvider<'msg>>().Dispatch))
            .AddMassTransit(fun (x: IBusRegistrationConfigurator) ->
                x.AddConsumer<MessageConsumer<'dto>>() |> ignore

                x.UsingRabbitMq(fun (ctx: IBusRegistrationContext) (cfg: IRabbitMqBusFactoryConfigurator) ->
                    cfg.Host(
                        "localhost",
                        "/",
                        fun h ->
                            h.Username("guest")
                            h.Password("guest")
                    )

                    cfg.ConfigureEndpoints(ctx)))
            .AddMassTransitTextWriterLogger()

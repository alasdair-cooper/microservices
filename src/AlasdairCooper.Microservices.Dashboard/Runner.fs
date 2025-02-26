namespace AlasdairCooper.Microservices.Dashboard

open System
open System.Threading
open Avalonia.Threading
open MassTransit
open Microsoft.Extensions.DependencyInjection

module Runner =
    let runner (services: IServiceCollection) =
        let start dispatch =
            services.AddDispatchProvider(dispatch) |> ignore

            let serviceProvider = services.BuildServiceProvider()

            let depot = serviceProvider.GetRequiredService<IBusDepot>()
            let cts = new CancellationTokenSource()

            Dispatcher.UIThread.InvokeAsync(fun _ -> depot.Start(cts.Token) |> Async.AwaitTask)
            |> ignore

            { new IDisposable with
                member _.Dispose() =
                    Dispatcher.UIThread.InvokeAsync(fun _ -> depot.Stop(cts.Token) |> Async.AwaitTask)
                    |> ignore }

        start
        
    let subscribe services _ = [ [ "runner" ], runner services ]


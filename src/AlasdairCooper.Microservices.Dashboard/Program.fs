namespace AlasdairCooper.Microservices.Dashboard

open System
open System.Threading
open AlasdairCooper.Microservices.Dashboard.Counter
open Avalonia
open Avalonia.Controls.ApplicationLifetimes
open Avalonia.FuncUI.Elmish
open Avalonia.Themes.Fluent
open Avalonia.FuncUI.Hosts
open Avalonia.Threading
open Elmish
open MassTransit
open Microsoft.Extensions.DependencyInjection

type MainWindow() as this =
    inherit HostWindow()
    
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

    do
        let services = ServiceCollection()
        
        services.AddMessageConsumer(Msg.Tick) |> ignore
        
        base.Title <- "Counter Example"
        
        let subscribe services _ =  [ [ "runner" ], runner services ]

        Program.mkSimple init update view
        |> Program.withHost this
        |> Program.withConsoleTrace
        |> Program.withSubscription (services |> subscribe)
        |> Program.run
        
type App() =
    inherit Application()

    override this.Initialize() =
        this.Styles.Add(FluentTheme())
        this.RequestedThemeVariant <- Styling.ThemeVariant.Dark

    override this.OnFrameworkInitializationCompleted() =
        match this.ApplicationLifetime with
        | :? IClassicDesktopStyleApplicationLifetime as desktopLifetime -> desktopLifetime.MainWindow <- MainWindow()
        | _ -> ()

module Program =

    [<EntryPoint>]
    let main (args: string[]) =
        AppBuilder
            .Configure<App>()
            .UsePlatformDetect()
            .UseSkia()
            .StartWithClassicDesktopLifetime(args)

namespace AlasdairCooper.Microservices.Dashboard

open AlasdairCooper.Microservices.Dashboard.Dashboard
open Avalonia
open Avalonia.Controls.ApplicationLifetimes
open Avalonia.FuncUI.Elmish
open Avalonia.Themes.Fluent
open Avalonia.FuncUI.Hosts
open Elmish
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection

type MainWindow() as this =
    inherit HostWindow()

    let initServiceCollection =
        let config = ConfigurationBuilder().AddJsonFile("appsettings.json").Build()
        ServiceCollection().AddSingleton(config :> IConfiguration)

    do
        let subscribe =
            initServiceCollection
                .AddMassTransit()
                .AddMessageConsumer(Msg.DateTime)
                .AddMessageConsumer(Msg.Log)
                .AddMessageConsumer(Msg.SystemInfo)
                .AddMessageConsumer(Msg.Ping)
            |> Runner.subscribe

        base.Title <- "Dashboard"

        Program.mkSimple init update view
        |> Program.withHost this
        |> Program.withConsoleTrace
        |> Program.withSubscription subscribe
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

namespace AlasdairCooper.Microservices.Dashboard

open System
open System.Linq
open System.Collections.Immutable
open AlasdairCooper.Microservices.Shared
open Avalonia.Controls
open Avalonia.FuncUI.DSL
open Avalonia.FuncUI.Types
open Avalonia.Layout
open LiveChartsCore.SkiaSharpView
open LiveChartsCore.SkiaSharpView.Avalonia
open LiveChartsCore.SkiaSharpView.Drawing.Geometries

module Dashboard =
    type Model =
        { Count: int
          DateTime: DateTime option
          CpuTemp: float option
          Logs: LogMessage list
          Pings: ImmutableQueue<int64> }

    let init () =
        { Count = 0
          DateTime = None
          CpuTemp = None
          Logs = []
          Pings = ImmutableQueue.Create() }

    type Msg =
        | Increment
        | Decrement
        | DateTime of DateTimeMessage
        | SystemInfo of SystemInfoMessage
        | Ping of NetworkPingMessage
        | Log of LogMessage

    let update msg (model: Model) =
        match msg with
        | Increment -> { model with Count = model.Count + 1 }
        | Decrement -> { model with Count = model.Count - 1 }
        | DateTime message ->
            { model with
                DateTime = Some message.DateTime }
        | SystemInfo message ->
            { model with
                CpuTemp = Some message.CpuTemp }
        | Log message ->
            { model with
                Logs = message :: model.Logs }
        | Ping networkPingMessage ->
            { model with
                Pings = model.Pings.Enqueue(networkPingMessage.RoundtripTime).EnsureBelowCapacity(100) }

    let view model dispatch =
        DockPanel.create
            [ DockPanel.children
                  [
                    // Button.create
                    //     [ Button.dock Dock.Bottom
                    //       Button.onClick (fun _ -> dispatch Decrement)
                    //       Button.content "-"
                    //       Button.horizontalAlignment HorizontalAlignment.Stretch
                    //       Button.horizontalContentAlignment HorizontalAlignment.Center ]
                    // Button.create
                    //     [ Button.dock Dock.Bottom
                    //       Button.onClick (fun _ -> dispatch Increment)
                    //       Button.content "+"
                    //       Button.horizontalAlignment HorizontalAlignment.Stretch
                    //       Button.horizontalContentAlignment HorizontalAlignment.Center ]
                    // TextBlock.create
                    //     [ TextBlock.dock Dock.Top
                    //       TextBlock.fontSize 48.0
                    //       TextBlock.verticalAlignment VerticalAlignment.Center
                    //       TextBlock.horizontalAlignment HorizontalAlignment.Center
                    //       TextBlock.text $"{model.Count}" ]
                    // TextBlock.create
                    //     [ TextBlock.dock Dock.Top
                    //       TextBlock.fontSize 48.0
                    //       TextBlock.verticalAlignment VerticalAlignment.Center
                    //       TextBlock.horizontalAlignment HorizontalAlignment.Center
                    //       TextBlock.text $"{model.CpuTemp}" ]
                    // TextBlock.create
                    //     [ TextBlock.dock Dock.Top
                    //       TextBlock.fontSize 48.0
                    //       TextBlock.verticalAlignment VerticalAlignment.Center
                    //       TextBlock.horizontalAlignment HorizontalAlignment.Center
                    //       TextBlock.text $"{model.CpuTemp}" ]
                    let data = model.Pings.ToArray() |> Array.map int64 |> Array.map Nullable
                    CartesianChart.create
                        [ CartesianChart.series [ LineSeries<Nullable<int64>>(data) ]
                          CartesianChart.easingFunction None
                          CartesianChart.height 300 ] ] ]
// StackPanel.create
//     [ model.Logs
//       |> List.map (fun x ->
//           TextBlock.create
//               [ match x with
//                 | Info message ->
//                     TextBlock.foreground "blue"
//                     TextBlock.text message
//                 | Error ``exception`` ->
//                     TextBlock.foreground "red"
//                     TextBlock.text ``exception``.Message ]
//           :> IView)
//       |> StackPanel.children ]
// TextBlock.create
//     [ TextBlock.dock Dock.Top
//       TextBlock.fontSize 48.0
//       TextBlock.verticalAlignment VerticalAlignment.Center
//       TextBlock.horizontalAlignment HorizontalAlignment.Center
//       TextBlock.text $"{model.DateTime}" ] ] ]

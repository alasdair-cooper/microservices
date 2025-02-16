namespace AlasdairCooper.Microservices.Dashboard

open System
open AlasdairCooper.Microservices.Shared.Messages
open Avalonia.Controls
open Avalonia.FuncUI.DSL
open Avalonia.Layout

module Counter =
    type CounterModel = { Count: int; DateTime: DateTime }

    let init () =
        { Count = 0
          DateTime = DateTime.MinValue }

    type Msg =
        | Increment
        | Decrement
        | Tick of TestMessage

    let update msg (model: CounterModel) =
        match msg with
        | Tick message ->
            { model with
                DateTime = message.DateTime }
        | Increment -> { model with Count = model.Count + 1 }
        | Decrement -> { model with Count = model.Count - 1 }

    let view model dispatch =
        DockPanel.create
            [ DockPanel.children
                  [ Button.create
                        [ Button.dock Dock.Bottom
                          Button.onClick (fun _ -> dispatch Decrement)
                          Button.content "-"
                          Button.horizontalAlignment HorizontalAlignment.Stretch
                          Button.horizontalContentAlignment HorizontalAlignment.Center ]
                    Button.create
                        [ Button.dock Dock.Bottom
                          Button.onClick (fun _ -> dispatch Increment)
                          Button.content "+"
                          Button.horizontalAlignment HorizontalAlignment.Stretch
                          Button.horizontalContentAlignment HorizontalAlignment.Center ]
                    TextBlock.create
                        [ TextBlock.dock Dock.Top
                          TextBlock.fontSize 48.0
                          TextBlock.verticalAlignment VerticalAlignment.Center
                          TextBlock.horizontalAlignment HorizontalAlignment.Center
                          TextBlock.text (string model.Count) ]
                    TextBlock.create
                        [ TextBlock.dock Dock.Top
                          TextBlock.fontSize 48.0
                          TextBlock.verticalAlignment VerticalAlignment.Center
                          TextBlock.horizontalAlignment HorizontalAlignment.Center
                          TextBlock.text (string model.DateTime) ] ] ]
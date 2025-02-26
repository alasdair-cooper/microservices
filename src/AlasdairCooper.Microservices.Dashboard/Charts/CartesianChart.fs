namespace AlasdairCooper.Microservices.Dashboard

open System
open Avalonia.FuncUI.Builder
open Avalonia.FuncUI.DSL
open Avalonia.FuncUI.Types
open LiveChartsCore
open LiveChartsCore.Kernel.Sketches
open LiveChartsCore.SkiaSharpView.Avalonia

[<AutoOpen>]
module CartesianChart =
    let create (attrs: IAttr<CartesianChart> list): IView<CartesianChart> =
        ViewBuilder.Create<CartesianChart>(attrs)

    type CartesianChart with
        static member xAxes<'t when 't :> CartesianChart>(axes: ICartesianAxis list) : IAttr<'t> =
            AttrBuilder<'t>.CreateProperty(CartesianChart.XAxesProperty, axes, ValueNone)

        static member yAxes<'t when 't :> CartesianChart>(axes: ICartesianAxis list) : IAttr<'t> =
            AttrBuilder<'t>.CreateProperty(CartesianChart.YAxesProperty, axes, ValueNone)

        static member series<'t when 't :> CartesianChart>(series: ISeries list) : IAttr<'t> =
            AttrBuilder<'t>.CreateProperty(CartesianChart.SeriesProperty, series, ValueNone)

        static member easingFunction<'t when 't :> CartesianChart>(easingFunction: (float32 -> float32) option) : IAttr<'t> =
            let func: Func<float32, float32> =
                match easingFunction with
                | Some x -> x
                | None -> null
            AttrBuilder<'t>.CreateProperty(CartesianChart.EasingFunctionProperty, func, ValueNone)

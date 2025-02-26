namespace AlasdairCooper.Microservices.Shared

open System
open System.Collections.Concurrent
open System.Collections.Immutable
open System.Linq
open System.Runtime.CompilerServices
open System.Threading.Tasks
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting

[<AbstractClass>]
type ScopedBackgroundService() =
    inherit BackgroundService()

type ScopedBackgroundServiceProcessor(serviceProvider: IServiceProvider) =
    let scope = serviceProvider.CreateAsyncScope()

    interface IHostedService with
        member this.StartAsync(cancellationToken) =
            scope.ServiceProvider.GetServices<ScopedBackgroundService>()
            |> Seq.map _.StartAsync(cancellationToken)
            |> Seq.map Async.AwaitTask
            |> Async.Parallel
            |> Async.StartAsTask
            :> Task

        member this.StopAsync(cancellationToken) =
            scope.ServiceProvider.GetServices<ScopedBackgroundService>()
            |> Seq.map _.StopAsync(cancellationToken)
            |> Seq.map Async.AwaitTask
            |> Async.Parallel
            |> Async.StartAsTask
            :> Task

    interface IAsyncDisposable with
        member this.DisposeAsync() = scope.DisposeAsync()

type ImmutableQueueExtensions =
    [<Extension>]
    static member EnsureBelowCapacity(queue: ImmutableQueue<'a>, capacity: int) =
        if queue.Count() > capacity then
            queue.Dequeue().EnsureBelowCapacity(capacity)
        else
            queue

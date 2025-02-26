namespace AlasdairCooper.Microservices.Shared

open System
open System.ComponentModel.DataAnnotations

[<AutoOpen>]
module Messaging =

    type DateTimeMessage = { DateTime: DateTime }

    type SystemInfoMessage = { CpuTemp: float }

    type NetworkPingMessage = { RoundtripTime: int64 }

    type LogMessage =
        | Info of string
        | Error of Exception

    type MessagingOptions() =

        [<Required>]
        member val Host = "" with get, set

        [<Required>]
        member val VirtualHost = "" with get, set

        [<Required>]
        member val Username = "" with get, set

        [<Required>]
        member val Password = "" with get, set

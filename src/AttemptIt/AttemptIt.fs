[<AutoOpen>]
module AttemptIt

open System.Diagnostics

type Result<'TSuccess, 'TFail> =
    | Success of 'TSuccess
    | Fail of 'TFail

let internal either successTrack failTrack result =
    match result with
    | Success s -> successTrack s
    | Fail f -> failTrack f

let internal bind successTrack = either successTrack Fail
let internal fail failTrack result = either Success failTrack result
let internal lift x = Success x

type AttemptBuilder() =
    member this.Bind(m, success) = bind success m
    member this.For(m, success) = bind success m
    member this.Return(x) = Success x
    member this.ReturnFrom(x:Result<_,_>) = x
    member this.Combine(v, f:unit -> _) = bind f v
    member this.TryWith(body, handler) =
        try body() |> this.ReturnFrom
        with e -> (handler e) |> Fail
    member this.TryFinally(body, compensation) =
        try this.ReturnFrom(body())
        finally compensation()
    member this.Yield(x) = Success x
    member this.YieldFrom(x) = x
    member this.Run m = m()
    member this.Delay(f) = f
    member this.Zero = Success()

    [<CustomOperation("either", MaintainsVariableSpace = true)>]
    member this.Either(m, success, failure) = either success failure m

    [<CustomOperation("fail", MaintainsVariableSpace = true)>]
    member this.Fail(m, failure) = fail failure m

let attempt = AttemptBuilder()
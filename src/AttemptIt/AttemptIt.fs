[<AutoOpen>]
module AttemptIt

open System

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

type Attempt =
    static member Catch (handler:exn -> Result<_,_>) (x: _ -> Result<_,_>) = fun _ -> try x() with e -> handler e
    static member Run (x: _ -> Result<_,_>) = x()

type AttemptBuilder() =
    member this.Bind(m, success) = bind success m
    member this.Return(x) = Success x
    member this.ReturnFrom(x:Result<_,_>) = x
    member this.Combine(v, f) = bind f v
    member this.TryWith(body, handler) =
        try body
        with e -> handler e |> Fail
    member this.TryFinally(body, compensation) =
        try body |> this.ReturnFrom
        finally compensation()

    member this.Using(res:#IDisposable, body) =
        this.TryFinally(body res, fun () -> match res with null -> () | disp -> disp.Dispose())

    member this.While(guard, f) =
        if not (guard()) then this.Zero() else
        this.Bind(f(), fun _ -> this.While(guard, f))

    member this.For(sequence:seq<_>, body) =
        this.Using(sequence.GetEnumerator(), fun enum -> this.While(enum.MoveNext, this.Delay(fun () -> body enum.Current)))

    member this.Yield(x) = Success x
    member this.YieldFrom(x) = x
    member this.Delay(f) = f
    member this.Zero() = Success()
//
//    [<CustomOperation("either", MaintainsVariableSpace = true)>]
//    member this.Either([<ProjectionParameter>]m, success, failure) = either success failure m
//
//    [<CustomOperation("fail", MaintainsVariableSpace = true)>]
//    member this.Fail([<ProjectionParameter>]m, failure) = fail failure m

let attempt = AttemptBuilder()
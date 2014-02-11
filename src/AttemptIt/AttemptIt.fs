[<AutoOpen>]
module AttemptIt

open System.Diagnostics

type Result<'TSuccess,'TFail> =
    | Success of 'TSuccess
    | Fail of 'TFail

let internal either successTrack failTrack result =
    match result with
    | Success s -> successTrack s
    | Fail f -> failTrack f

let internal tryEither exceptionTrack successTrack failTrack result =
    try
     either successTrack failTrack result
    with
    | exn -> exceptionTrack exn

let internal bind successTrack = either successTrack Fail
let internal tryBind exceptionTrack successTrack = tryEither exceptionTrack successTrack Fail
let internal fail failTrack result =  either Success failTrack result
let internal lift x = Success x

type AttemptBuilder() =
//type AttemptBuilder<'TSuccess, 'TFail>(exceptionHandler:exn -> Result<'TSuccess,'TFail>) =
    member this.Bind(m, success) = bind success m
    member this.Bind((m:'a, exceptionHandler), (success:'a -> Result<'b, 'c>)) = tryBind exceptionHandler (success >> lift) m
    member this.Bind((m, exceptionHandler), success) = tryBind exceptionHandler success m
    member this.For(m, success) = bind success m
    member this.Return(x) = Success x
    member this.ReturnFrom(x) = x
    member this.Yield(x) = Success x
    member this.YieldFrom(x) = x
//    member this.Run m = m
//    member this.Delay f = f()
    member this.Zero = Success ()

    [<CustomOperation("either", MaintainsVariableSpace = true)>]
    member this.Either(m, success, failure) = either success failure m

    [<CustomOperation("fail", MaintainsVariableSpace = true)>]
    member this.Fail(m, failure) = fail failure m

let attempt = AttemptBuilder()
[<AutoOpen>]
module AttemptIt.Attempt

open System

type Result<'TSuccess, 'TFail> =
    | Success of 'TSuccess
    | Fail of 'TFail

type Attempt<'TSuccess, 'TFail> = (unit -> Result<'TSuccess,'TFail>)

let internal succeed x = (fun () -> Success x)
let internal failed x = (fun () -> Fail x)
let internal runAttempt (a:Attempt<_,_>) = a()
let internal delay f = (fun () -> runAttempt(f()))

let internal either successTrack failTrack (input:Attempt<_,_>):Attempt<_,_> =
    match runAttempt input with
        | Success s -> successTrack s
        | Fail f -> failTrack f

let internal bind successTrack = either successTrack failed
let internal fail failTrack result = either succeed failTrack result

type Attempt =
    static member Catch (handler:exn -> Result<_,_>) (x:Attempt<_,_>):Attempt<_,_> = fun _ -> try runAttempt x with e -> handler e
    static member Run x = runAttempt x

type AttemptBuilder() =
    member this.Bind(m:Attempt<_,_>, success) = bind success m
    member this.Bind(m:Result<_,_>, success) = bind success (fun () -> m)
    member this.Return(x) : Attempt<_,_>  = succeed x
    member this.ReturnFrom(x:Attempt<_,_>) = x
    member this.Combine(v, f):Attempt<_,_> = bind f v
    member this.Yield(x) = Success x
    member this.YieldFrom(x) = x
    member this.Delay(f):Attempt<_,_> = delay f
    member this.Zero() : Attempt<_,_> = succeed()

let attempt = AttemptBuilder()
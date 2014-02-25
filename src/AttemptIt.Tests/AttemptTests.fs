module AttemptIt.Tests

open AttemptIt
open FsUnit.Xunit
open System
open Xunit

let successValue x =
    match x with
    | Success s -> s
    | Fail f -> raise (ArgumentException(sprintf "%A failed" x))

let failValue x =
    match x with
    | Success s -> raise (ArgumentException(sprintf "%A should not have been successful" x))
    | Fail f -> f

let isSuccess x =
    match x with
    | Success s -> true
    | Fail f -> false

let isFailure x = isSuccess x |> not

[<Fact>]
let ``An empty attempt should be successful``() =
    attempt { return () }
    |> Attempt.Run
    |> isSuccess
    |> should be True

[<Fact>]
let ``An attempt with an exception and no catch should still throw``() =
    (fun () ->
    attempt {
        raise (Exception("Should not be caught"))
        return ()
    }
    |> Attempt.Run
    |> ignore)
    |> should throw typeof<Exception>

[<Fact>]
let ``An attempt with an exception and a catch shoudld return Fail``() =
    let exceptionHandler (e : exn) = Fail e.Message
    attempt {
        raise (Exception("Should be caught"))
        return ()
    }
    |> Attempt.Catch exceptionHandler
    |> Attempt.Run
    |> isFailure
    |> should be True

[<Fact>]
let ``A series of success attempts should result in an overall success``() =
    let successfullFunc x = attempt { return x + 1 }
    let overallAttempt = attempt { let! a = successfullFunc 1
                                   let! b = successfullFunc a
                                   let! c = successfullFunc b
                                   return! successfullFunc c }
    overallAttempt
    |> Attempt.Run
    |> successValue
    |> should equal 5

[<Fact>]
let ``A series of success attempts that fail should result in an overall fail``() =
    let successfullFunc x = attempt { return x + 1 }
    let failFunc x = Fail x
    let overallAttempt = attempt { let! a = successfullFunc 1
                                   let! b = successfullFunc a
                                   let! c = failFunc b
                                   return! successfullFunc c }
    overallAttempt
    |> Attempt.Run
    |> failValue
    |> should equal 3
AttemptIt
=========

F# computation expression based on railway programming http://fsharpforfunandprofit.com/posts/recipe-part2/

## Build Status
[![Build Status](https://api.travis-ci.org/TWith2Sugars/AttemptIt.png)](https://travis-ci.org/TWith2Sugars/AttemptIt)

Types
-----
* Attempt<'TSuccess, 'TFail> represents an attempt at doing something
* Result<'TSuccess, 'TFail> represents the result of an attempt

Results are a DU of Success and Fail (both generic) and

Syntax
------

**Basic usage**

```F#
let doThis = attempt {
    let! x = someFunc()
    let! y = someOtherFunc y
    return! finalFunc y
}
```

**Catching Exceptions**

```F#
let exceptionHandler (e:exn) -> Fail e.Message
let tryThis = Attempt.Catch exceptionHandler doThis
```


**Evaluating**

```F#
let result = Attempt.Run tryThis
```



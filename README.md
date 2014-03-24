AttemptIt
=========

F# computation expression based on railway programming http://fsharpforfunandprofit.com/posts/recipe-part2/

## Build Status
[![Build status](https://ci.appveyor.com/api/projects/status/8886nui850npmjbg)](https://ci.appveyor.com/project/TWith2Sugars/attemptit)

## Nuget
https://www.nuget.org/packages/AttemptIt

Types
-----
* Attempt<'TSuccess, 'TFail> represents an attempt at doing something
* Result<'TSuccess, 'TFail> represents the result of an attempt

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



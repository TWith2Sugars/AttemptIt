// include Fake lib
#r "tools/FAKE/tools/FakeLib.dll"
open System
open Fake

// Properties
let buildDir = "./build/"
let testDir  = "./test/"

// Targets
Target "RestorePackage" RestorePackages

Target "Clean" (fun _ ->
    CleanDirs [buildDir; testDir]
)

Target "Build" (fun _ ->
    !! "src/**/*.fsproj"
      |> Seq.filter (fun p -> p.Contains("Test") |> not)
      |> MSBuildRelease buildDir "Build"
      |> Log "Build-Output: "
)

Target "BuildTests" (fun _ ->
    !! "src/**/*Test*.fsproj"
      |> MSBuildDebug testDir "Build"
      |> Log "BuildTests-Output: "
)

let testDlls = !! (testDir + "*Test*.dll")

Target "Test" (fun _ ->
    testDlls
        |> xUnit (fun p -> 
            {p with 
                ShadowCopy = false;
                HtmlOutput = true;
                XmlOutput = true;
                OutputDir = testDir })
)

Target "Default" (fun _ ->
    trace "Hello World from FAKE"
)

// Dependencies
"Clean"
  ==> "RestorePackage"
  ==> "Build"
  =?> ("BuildTests", isLocalBuild)
  =?> ("Test", isLocalBuild)
  ==> "Default"

// start build
RunTargetOrDefault "Default"
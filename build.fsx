// include Fake lib
#r "tools/FAKE/tools/FakeLib.dll"
open System
open Fake

// Properties
let buildDir = "./build/"
let testDir  = "./test/"

let isAppVeyorBuild = environVar "APPVEYOR" <> null

/// http://www.appveyor.com/docs2/environment-variables
type AppVeyorEnv =
    static member ApiUrl = environVar "APPVEYOR_API_URL"
    static member ProjectId = environVar "APPVEYOR_PROJECT_ID"
    static member ProjectName = environVar "APPVEYOR_PROJECT_NAME"
    static member ProjectSlug = environVar "APPVEYOR_PROJECT_SLUG"
    static member BuildFolder = environVar "APPVEYOR_BUILD_FOLDER"
    static member BuildId = environVar "APPVEYOR_BUILD_ID"
    static member BuildNumber = environVar "APPVEYOR_BUILD_NUMBER"
    static member BuildVersion = environVar "APPVEYOR_BUILD_VERSION"
    static member JobId = environVar "APPVEYOR_JOB_ID"
    static member RepoProvider = environVar "APPVEYOR_REPO_PROVIDER"
    static member RepoScm = environVar "APPVEYOR_REPO_SCM"
    static member RepoName = environVar "APPVEYOR_REPO_NAME"
    static member RepoBranch = environVar "APPVEYOR_REPO_BRANCH"
    static member RepoCommit = environVar "APPVEYOR_REPO_COMMIT"
    static member RepoCommitAuthor = environVar "APPVEYOR_REPO_COMMIT_AUTHOR"
    static member RepoCommitAuthorEmail = environVar "APPVEYOR_REPO_COMMIT_AUTHOR_EMAIL"
    static member RepoCommitTimestamp = environVar "APPVEYOR_REPO_COMMIT_TIMESTAMP"
    static member RepoCommitMessage = environVar "APPVEYOR_REPO_COMMIT_MESSAGE"

Target "AppVeyor" (fun _ ->
    logfn "APPVEYOR: %s" (environVar "APPVEYOR")
    logfn "CI: %s" (environVar "CI")
    logfn "ApiUrl: %s" AppVeyorEnv.ApiUrl
    logfn "ProjectId: %s" AppVeyorEnv.ProjectId
    logfn "ProjectName: %s" AppVeyorEnv.ProjectName
    logfn "ProjectSlug: %s" AppVeyorEnv.ProjectSlug
    logfn "BuildFolder: %s" AppVeyorEnv.BuildFolder
    logfn "BuildId: %s" AppVeyorEnv.BuildId
    logfn "BuildNumber: %s" AppVeyorEnv.BuildNumber
    logfn "BuildVersion: %s" AppVeyorEnv.BuildVersion
    logfn "JobId: %s" AppVeyorEnv.JobId
    logfn "RepoProvider: %s" AppVeyorEnv.RepoProvider
    logfn "RepoScm: %s" AppVeyorEnv.RepoScm
    logfn "RepoName: %s" AppVeyorEnv.RepoName
    logfn "RepoBranch: %s" AppVeyorEnv.RepoBranch
    logfn "RepoCommit: %s" AppVeyorEnv.RepoCommit
    logfn "RepoCommitAuthor: %s" AppVeyorEnv.RepoCommitAuthor
    logfn "RepoCommitAuthorEmail: %s" AppVeyorEnv.RepoCommitAuthorEmail
    logfn "RepoCommitTimestamp: %s" AppVeyorEnv.RepoCommitTimestamp
    logfn "RepoCommitMessage: %s" AppVeyorEnv.RepoCommitMessage
)


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
  =?> ("AppVeyor", isAppVeyorBuild)
  ==> "Build"
  ==> "BuildTests"
  ==> "Test"
  ==> "Default"

// start build
RunTargetOrDefault "Default"
open System

#if !FAKE
  #r "netstandard" // .NET
  #r "Facades/netstandard" // Mono
#endif

#r "paket:
nuget Fake.Core.Target
//"

#load "./.fake/build.fsx/intellisense.fsx"

open Fake.Core

Target.create "Build" (fun _ ->
  Trace.trace "Build target is running"
)

Target.create "Test" (fun _ ->
  Trace.trace "Test target is running"
)

Target.create "Package" (fun _ ->
  Trace.trace "Package target is running"
)

Target.create "Deploy" (fun _ ->
  Trace.trace "Deploy target is running"
)

open Fake.Core.TargetOperators

"Build"
  ==> "Test"
  ==> "Package"
  ==> "Deploy"

Target.runOrDefault "Build"

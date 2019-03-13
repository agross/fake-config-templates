open System
#r "paket:
nuget Fake.Core.Target
nuget Fake.DotNet.MsBuild
//"

#load "./.fake/build.fsx/intellisense.fsx"

open Fake.Core
open Fake.DotNet

Target.create "Build" (fun _ ->
  Trace.trace "Build target is running"

  let buildMode = Environment.environVarOrDefault "buildMode" "Release"
  let setParams (defaults:MSBuildParams) =
    { defaults with
        Verbosity = Some(Quiet)
        Targets = ["Build"]
        Properties =
          [
            "Optimize", "True"
            "DebugSymbols", "True"
            "Configuration", buildMode
          ]
    }
  MSBuild.build setParams "./MySolution.sln"
)

/// Target.Create("Build", (_) => { Console.WriteLine("Build target ist running") })

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

open System
open System.IO
open Fake.Core
open Fake.IO
open Fake.IO.Globbing.Operators
open Fake.BuildServer

#if !FAKE
  #r "netstandard" // .NET
  #r "Facades/netstandard" // Mono
#endif

#r "paket:
nuget Fake.Core.Target
nuget Fake.IO.FileSystem
nuget Fake.BuildServer.TeamCity
nuget FuManchu
//"

#load "./.fake/build.fsx/intellisense.fsx"

#load "./config.fsx"
let Config = Config.Config
Trace.tracefn "Configuration:\n%A" Config

BuildServer.install [TeamCity.Installer]

Target.create "Template" (fun _ ->
  Trace.trace "Template target is running"

  let removeExtension template =
    let file = Path.GetFileNameWithoutExtension template
    Path.combine (Path.getDirectory template) file

  let render config templateFile =
    let template = File.readAsString templateFile

    FuManchu.Handlebars.CompileAndRun (templateFile, template, config)

  !! "**/*.template"
  |> Seq.iter (fun f ->
    let targetFilename = removeExtension f

    Trace.tracefn "Creating %s -> %s" f targetFilename

    render Config f
    |> File.writeString false targetFilename
  )
)

Target.create "Build" (fun _ ->
  Trace.trace "Build target is running"

  Trace.publish ImportData.BuildArtifact "bin/**/*.dll => dlls.zip"
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

"Template"
  ==> "Build"
  ==> "Test"
  ==> "Package"
  ==> "Deploy"

Target.runOrDefault "Build"

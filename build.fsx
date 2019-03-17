open System
open System.IO
open Fake.Core
open Fake.IO
open Fake.IO.Globbing.Operators

#if !FAKE
  #r "netstandard" // .NET
  #r "Facades/netstandard" // Mono
#endif

#r "paket:
nuget Fake.Core.Target
nuget Fake.IO.FileSystem
nuget Fue
// Fue needs exactly this version.
nuget HtmlAgilityPack 1.5.2
//"

#load "./.fake/build.fsx/intellisense.fsx"

#load "./config.fsx"
let Config = Config.Config

Target.create "Template" (fun _ ->
  Trace.trace "Template target is running"

  let removeExtension template =
    let file = Path.GetFileNameWithoutExtension template
    Path.combine (Path.getDirectory template) file

  let saveFile mapFilename filename contents =
    let targetFilename = mapFilename filename
    Trace.trace (sprintf "Creating %s -> %s" filename targetFilename)
    File.WriteAllText (targetFilename, contents)

  let render model templateFile =
    Fue.Data.init
    |> Fue.Data.add "config" model
    |> Fue.Compiler.fromFile templateFile

  Trace.tracefn "Configuration\n%O" Config

  !! "**/*.template"
  |> Seq.iter (fun f ->
    render Config f
    |> saveFile removeExtension f
  )
)

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

"Template"
  ==> "Build"
  ==> "Test"
  ==> "Package"
  ==> "Deploy"

Target.runOrDefault "Build"

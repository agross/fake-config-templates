open System
open Fake.IO

#if !FAKE
  #r "netstandard" // .NET
  #r "Facades/netstandard" // Mono
#endif

#r "paket:
nuget Fake.Core.Target
nuget Fake.IO.FileSystem
//"

#load "./.fake/build.fsx/intellisense.fsx"

open Fake.Core
open Fake.IO
open Fake.IO.Globbing.Operators

Target.create "Template" (fun _ ->
  Trace.trace "Template target is running"

  let removeExtension template =
    let file = System.IO.Path.GetFileNameWithoutExtension template
    Path.combine (Path.getDirectory template) file

  let saveFiles mapFilename =
    Seq.iter (fun (filename, contents) ->
      let targetFilename = mapFilename filename
      Trace.trace (sprintf "Creating %s -> %s" filename targetFilename)
      File.write false targetFilename (Seq.toList contents)
    )

  let replaceInFiles replacements files =
    files
    |> Templates.load
    |> Templates.replaceKeywords replacements
    |> saveFiles removeExtension

  let replacements = [
    ("the.key", "the value")
  ]
  replaceInFiles replacements (!! "**/*.template")
  ()
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

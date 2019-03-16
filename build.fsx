open System

#if !FAKE
  #r "netstandard" // .NET
  #r "Facades/netstandard" // Mono
#endif

#r "paket:
nuget Fake.Core.Target
nuget Fake.IO.FileSystem
nuget Microsoft.Extensions.Configuration
nuget Microsoft.Extensions.Configuration.Yaml
// Microsoft.Extensions.Configuration.Yaml needs exactly this version.
nuget YamlDotNet 4.2.1
nuget Microsoft.Extensions.Configuration.EnvironmentVariables
nuget Fue
// Fue needs exactly this version.
nuget HtmlAgilityPack 1.5.2
//"

#load "./.fake/build.fsx/intellisense.fsx"

open System.IO
open Fake.Core
open Fake.IO
open Fake.IO.Globbing.Operators

open Microsoft.Extensions.Configuration
open Microsoft.Extensions.Configuration.Yaml

let Config =
  let builder = ConfigurationBuilder ()
  builder.AddYamlFile (__SOURCE_DIRECTORY__ + "/config/default.yaml") |> ignore

  let local = __SOURCE_DIRECTORY__ + "/config/local.yaml"
  if File.exists local then
    builder.AddYamlFile (local) |> ignore
  builder.Build()

Config.AsEnumerable()
|> Seq.map (fun kvp -> sprintf "%s = %s" kvp.Key kvp.Value)
|> Seq.sort
|> Seq.iter Trace.trace

Target.create "Template" (fun _ ->
  Trace.trace "Template target is running"

  let removeExtension template =
    let file = Path.GetFileNameWithoutExtension template
    Path.combine (Path.getDirectory template) file

  let saveFile mapFilename filename contents =
    let targetFilename = mapFilename filename
    Trace.trace (sprintf "Creating %s -> %s" filename targetFilename)
    File.WriteAllText (targetFilename, contents)

  let render (config:IConfigurationRoot) templateFile =
    let mutable init = Fue.Data.init

    config.AsEnumerable()
    |> Seq.iter (fun kvp ->
      init <- Fue.Data.add kvp.Key kvp.Value init
    )

    Fue.Compiler.fromFile templateFile init

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

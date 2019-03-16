open Fake.Core
open Fake.IO
open Fake.IO.Globbing.Operators
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.Configuration.Yaml
open System.IO
open System.Linq

#if !FAKE
  #r "netstandard" // .NET
  #r "Facades/netstandard" // Mono
#endif

#r "paket:
nuget Fake.Core.Target
nuget Fake.IO.FileSystem
nuget Microsoft.Extensions.Configuration
nuget Microsoft.Extensions.Configuration.Binder
nuget Microsoft.Extensions.Configuration.Yaml
// Microsoft.Extensions.Configuration.Yaml needs exactly this version.
nuget YamlDotNet 4.2.1
nuget Microsoft.Extensions.Configuration.EnvironmentVariables
nuget FuManchu
//"

#load "./.fake/build.fsx/intellisense.fsx"

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

  let render (config:IConfigurationRoot) templateFile =
    let template = File.readAsString templateFile

    FuManchu.Handlebars.RegisterHelper ("list", (fun o ->
      let key = string o.Data

      try
        config.AsEnumerable()
        |> Seq.find (fun kvp -> kvp.Key = key)
        |> ignore

        let result:string list = config.GetValue (key)
        System.String.Join (",", result)
      with
      | :? System.Collections.Generic.KeyNotFoundException ->
        failwithf "Key %s not found while rendering %s" key templateFile
      )
    )

    let data =
      config.AsEnumerable()
           .ToDictionary((fun kvp -> kvp.Key), (fun kvp -> kvp.Value))

    FuManchu.Handlebars.CompileAndRun (templateFile, template, data)

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

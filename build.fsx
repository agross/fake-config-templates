open System

#if !FAKE
  #r "netstandard" // .NET
  #r "Facades/netstandard" // Mono
#endif

#r "paket:
nuget Fake.Core.Target
nuget Fake.IO.FileSystem
nuget DotLiquid
//"

#load "./.fake/build.fsx/intellisense.fsx"

#load "./config/local.fsx"
let Config = Configuration.Default.Config

open System.IO
open Fake.Core
open Fake.IO
open Fake.IO.Globbing.Operators

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
    // https://github.com/SuaveIO/suave/blob/master/src/Suave.DotLiquid/Library.fs
    let safe =
      let o = obj()
      fun f -> lock o f

    /// Given a type which is an F# record containing seq<_>, list<_>, array<_>, option and
    /// other records, register the type with DotLiquid so that its fields are accessible
    let tryRegisterTypeTree =
      let registered = System.Collections.Generic.Dictionary<_, _>()
      let rec loop ty =
        if not (registered.ContainsKey ty) then
          if Reflection.FSharpType.IsRecord ty then
            let fields = Reflection.FSharpType.GetRecordFields ty
            DotLiquid.Template.RegisterSafeType(ty, [| for f in fields -> f.Name |])
            for f in fields do loop f.PropertyType
          elif ty.IsGenericType then
            let t = ty.GetGenericTypeDefinition()
            if t = typedefof<seq<_>> || t = typedefof<list<_>>  then
              loop (ty.GetGenericArguments().[0])
            elif t = typedefof<option<_>> then
              DotLiquid.Template.RegisterSafeType(ty, [|"Value"|])
              loop (ty.GetGenericArguments().[0])
          elif ty.IsArray then
            loop (ty.GetElementType())
          registered.[ty] <- true
      fun ty -> safe (fun () -> loop ty)

    tryRegisterTypeTree (model.GetType ())
    let view = File.readAsString templateFile
               |> DotLiquid.Template.Parse

    DotLiquid.Template.NamingConvention <- DotLiquid.NamingConventions.CSharpNamingConvention()

    model
    |> DotLiquid.Hash.FromAnonymousObject
    |> view.Render

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

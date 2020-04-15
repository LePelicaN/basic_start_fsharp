#r "paket: groupref build //"
#load "./.fake/build.fsx/intellisense.fsx"

#if !FAKE
#r "netstandard"
#r "Facades/netstandard" // https://github.com/ionide/ionide-vscode-fsharp/issues/839#issuecomment-396296095
#endif

open System

open Fake.Core
open Fake.DotNet
open Fake.IO
open Fake.IO.Globbing.Operators //enables !! and globbing

let buildDir = !! "./**/bin/"
let appReferences = !! "./**/*.fsproj"

Target.initEnvironment ()

let runDotNet cmd workingDir =
    let result =
        DotNet.exec (DotNet.Options.withWorkingDirectory workingDir) cmd ""
    if result.ExitCode <> 0 then failwithf "'dotnet %s' failed in %s" cmd workingDir

Target.create "Clean" (fun _ ->
  buildDir
  |> Shell.cleanDirs
)

Target.create "Restore" (fun _ ->
  appReferences
  |> Seq.iter (fun p ->
    let dir = System.IO.Path.GetDirectoryName p
    runDotNet "restore" dir
  )
)

Target.create "Build" (fun _ ->
  appReferences
  |> Seq.iter (fun p ->
    let dir = System.IO.Path.GetDirectoryName p
    runDotNet "build" dir
  )
)

open Fake.Core.TargetOperators

"Clean"
    ==> "Restore"
    ==> "Build"

Target.runOrDefaultWithArguments "Build"

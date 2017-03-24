#r "FakeLib.dll"

open System
open Fake
open Fake.NuGet.Install
open Fake.Testing.NUnit3

Target "Clean" (fun _ ->
    CleanDir "./build/release"
    CleanDir "./build/tests"
    CleanDir "./build/package"
    CleanDir "./build/merge"
)

Target "Restore" (fun _ ->
    RestorePackages()
)

Target "BuildApp" (fun _ ->
    !! "sources/**/leak.csproj"
      |> MSBuildRelease "./build/release/leak" "Build"
      |> Log "Application-Build-Output: "

    !! "sources/**/leak.connect.csproj"
      |> MSBuildRelease "./build/release/leak-direct" "Build"
      |> Log "Application-Build-Output: "

    !! "sources/**/leak.announce.csproj"
      |> MSBuildRelease "./build/release/leak-tracker" "Build"
      |> Log "Application-Build-Output: "
)

Target "BuildTests" (fun _ ->
    !! "sources/**/*.tests.csproj"
      |> MSBuildDebug "./build/tests" "Build"
      |> Log "Tests-Build-Output: "
)

Target "ExecuteTests" (fun _ ->
    "NUnit.Runners"
        |> NugetInstall (fun p ->
            { p with
                OutputDirectory = "./build/tools"
                ExcludeVersion = true})

    !! ("build/tests/*.Tests.dll")
        |> NUnit3 (fun p ->
            { p with
                ToolPath = findToolInSubPath "nunit3-console.exe" "build/tools"
                Agents = Some 1
                Workers = Some 1 })
)

Target "MergeApp" (fun _ ->
    "ILRepack"
        |> NugetInstall (fun p ->
            { p with
                OutputDirectory = "./build/tools"
                ExcludeVersion = true})

    let leak =
        ExecProcess (fun info ->
            info.FileName <- "./build" @@ "tools" @@ "ILRepack" @@ "tools" @@ "ILRepack.exe"
            info.WorkingDirectory <- "./build"
            info.Arguments <- sprintf "-target:exe -internalize -verbose -wildcards -lib:%s -out:%s %s %s" ("./release/leak") ("./merge" @@ "leak.exe") ("./release/leak/leak.exe") ("./release/leak/*.dll")
            ) (TimeSpan.FromMinutes 5.)

    let direct =
        ExecProcess (fun info ->
            info.FileName <- "./build" @@ "tools" @@ "ILRepack" @@ "tools" @@ "ILRepack.exe"
            info.WorkingDirectory <- "./build"
            info.Arguments <- sprintf "-target:exe -internalize -verbose -wildcards -lib:%s -out:%s %s %s" ("./release/leak-direct") ("./merge" @@ "leak-direct.exe") ("./release/leak-direct/leak-connect.exe") ("./release/leak-direct/*.dll")
            ) (TimeSpan.FromMinutes 5.)

    let tracker =
        ExecProcess (fun info ->
            info.FileName <- "./build" @@ "tools" @@ "ILRepack" @@ "tools" @@ "ILRepack.exe"
            info.WorkingDirectory <- "./build"
            info.Arguments <- sprintf "-target:exe -internalize -verbose -wildcards -lib:%s -out:%s %s %s" ("./release/leak-tracker") ("./merge" @@ "leak-tracker.exe") ("./release/leak-tracker/leak-announce.exe") ("./release/leak-tracker/*.dll")
            ) (TimeSpan.FromMinutes 5.)

    let core =
        ExecProcess (fun info ->
            info.FileName <- "./build" @@ "tools" @@ "ILRepack" @@ "tools" @@ "ILRepack.exe"
            info.WorkingDirectory <- "./build"
            info.Arguments <- sprintf "-target:library -verbose -wildcards -lib:%s -out:%s %s" ("./release/leak") ("./merge/Leak.Core.dll") ("./release/leak/Leak.*.dll")
            ) (TimeSpan.FromMinutes 5.)

    if leak <> 0 then failwithf "Error during ILRepack execution."
    if direct <> 0 then failwithf "Error during ILRepack execution."
    if tracker <> 0 then failwithf "Error during ILRepack execution."
    if core <> 0 then failwithf "Error during ILRepack execution."
)

Target "CreatePackage" (fun _ ->
    !! "build/merge/*.exe" -- "build/merge/*.pdb" -- "build/merge/*.xml"
        |> Zip "build/merge" ("build/package/leak-" + (getBuildParamOrDefault "version" "dev") + ".zip")

    NuGet (fun p ->
        { p with
            Version = (getBuildParamOrDefault "version" "1.0.0.dev")
            OutputPath = "./build/package"
            WorkingDir = "./build/merge"
            Dependencies = []
            Files = [( "Leak.Core.dll", Some "lib\\net45", None )]
            Publish = false }) "./build/build.nuspec")

Target "Default" (fun _ ->
    trace "Build completed."
)

"Clean"
    ==> "Restore"
    ==> "BuildApp"
    ==> "BuildTests"
    ==> "ExecuteTests"
    ==> "MergeApp"
    ==> "CreatePackage"
    ==> "Default"

RunTargetOrDefault "Default"
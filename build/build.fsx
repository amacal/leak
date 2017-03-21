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
      |> MSBuildRelease "./build/release" "Build"
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

    let result =
        ExecProcess (fun info ->
            info.FileName <- "./build" @@ "tools" @@ "ILRepack" @@ "tools" @@ "ILRepack.exe"
            info.WorkingDirectory <- "./build"
            info.Arguments <- sprintf "-target:exe -internalize -verbose -wildcards -lib:%s -out:%s %s %s" ("./release") ("./merge" @@ "Leak.exe") ("./release" @@ "Leak.exe") ("./release" @@ "*.dll")
            ) (TimeSpan.FromMinutes 5.)

    if result <> 0 then failwithf "Error during ILRepack execution."
)

Target "CreatePackage" (fun _ ->
     !! "build/merge/*.*" -- "build/merge/*.pdb" -- "build/merge/*.xml"
        |> Zip "build/merge" ("build/package/leak-" + (getBuildParamOrDefault "version" "dev") + ".zip")
)

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
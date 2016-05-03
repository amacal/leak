#r "FakeLib.dll"

open Fake
open Fake.NuGet.Install
open Fake.Testing.NUnit3

Target "Clean" (fun _ ->
    CleanDir "./build/release"
    CleanDir "./build/tests"
    CleanDir "./build/package"
)

Target "Restore" (fun _ ->
    RestorePackages()
    "NUnit.Runners"
        |> NugetInstall (fun p ->
            { p with
                OutputDirectory = "./build"})
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
    !! ("build/tests/*.Tests.dll")
        |> NUnit3 (fun p -> 
            { p with
                ToolPath = findToolInSubPath "nunit3-console.exe" "build" })
)

Target "CreatePackage" (fun _ ->
     !! "build/release/*.*" -- "build/release/*.pdb"
        |> Zip "build/release" "build/package/leak-1.1.zip"
)

Target "Default" (fun _ ->
    trace "Build completed."
)

"Clean" 
    ==> "Restore"
    ==> "BuildApp"
    ==> "BuildTests"
    ==> "ExecuteTests"
    ==> "CreatePackage"
    ==> "Default"    

RunTargetOrDefault "Default"
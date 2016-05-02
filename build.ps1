Invoke-WebRequest "http://nuget.org/nuget.exe" -OutFile "$PSScriptRoot\build\nuget.exe"

& "$PSScriptRoot\build\nuget.exe" "Install" "FAKE" "-OutputDirectory" "build" "-ExcludeVersion"
& "$PSScriptRoot\build\FAKE\tools\Fake.exe" "$PSScriptRoot\build\build.fsx"

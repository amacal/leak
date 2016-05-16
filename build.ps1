New-Item -ItemType Directory -Force -Path "$PSScriptRoot\build\tools"
Invoke-WebRequest "http://nuget.org/nuget.exe" -OutFile "$PSScriptRoot\build\tools\nuget.exe"

& "$PSScriptRoot\build\tools\nuget.exe" "Install" "FAKE" "-OutputDirectory" "build\tools" "-ExcludeVersion"
& "$PSScriptRoot\build\tools\FAKE\tools\Fake.exe" "$PSScriptRoot\build\build.fsx" version=$args

New-Item "$PSScriptRoot\build\tools" -Type Directory -ErrorAction Ignore
Invoke-WebRequest "https://dist.nuget.org/win-x86-commandline/latest/nuget.exe" -OutFile "$PSScriptRoot\build\tools\nuget.exe"

& "$PSScriptRoot\build\tools\nuget.exe" "Install" "FAKE" "-OutputDirectory" "build/tools" "-ExcludeVersion"
& "$PSScriptRoot\build\tools\FAKE\tools\Fake.exe" "$PSScriptRoot\build\build.fsx" version=$args

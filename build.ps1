Write-Host "build: Build started"

Push-Location $PSScriptRoot

if(Test-Path .\artifacts) {
	Write-Host "build: Cleaning .\artifacts"
	Remove-Item .\artifacts -Force -Recurse
}

# Get the branch from Appveyor or Travis or git
$branch = $NULL
if($env:APPVEYOR_REPO_BRANCH) {
    $branch = $env:APPVEYOR_REPO_BRANCH
}
if(!$branch -and $env:TRAVIS_BRANCH) {
    $branch = $env:TRAVIS_BRANCH
}
if(!$branch) {
    $branch = $(git symbolic-ref --short -q HEAD)
}
Write-Host "build: branch is $branch"
if(!$branch) {
    Write-Host "build: Error: Unable to detect branch"
    exit 1
}

$revision = @{ $true = "{0:00000}" -f [convert]::ToInt32("0" + $env:APPVEYOR_BUILD_NUMBER, 10); $false = "local" }[$env:APPVEYOR_BUILD_NUMBER -ne $NULL];
$suffix = @{ $true = ""; $false = "$($branch.Substring(0, [math]::Min(10,$branch.Length)))-$revision"}[$branch -eq "master" -and $revision -ne "local"]
$commitHash = $(git rev-parse --short HEAD)
$buildSuffix = @{ $true = "$($suffix)-$($commitHash)"; $false = "$($branch)-$($commitHash)" }[$suffix -ne ""]

Write-Host "build: Package version suffix is $suffix"
Write-Host "build: Build version suffix is $buildSuffix" 

foreach ($src in Get-ChildItem src/*) {
    Push-Location $src

    Write-Host "build: Packaging project in $src"

    & dotnet build -c Release --version-suffix=$buildSuffix

    if($suffix) {
        & dotnet pack -c Release --no-build -o ..\..\artifacts --version-suffix=$suffix
    } else {
        & dotnet pack -c Release --no-build -o ..\..\artifacts
    }
    if($LASTEXITCODE -ne 0) { exit 1 }

    Pop-Location
}

foreach ($src in Get-ChildItem sample/*) {
    Push-Location $src

    Write-Host "build: compiling project in $src"

    & dotnet build -c Release
    if($LASTEXITCODE -ne 0) { exit 1 }

    Pop-Location
}

foreach ($test in Get-ChildItem test/*Tests) {
    Push-Location $test

    Write-Host "build: Testing project in $test"

    & dotnet test -c Release
    if($LASTEXITCODE -ne 0) { exit 3 }

    Pop-Location
}

Pop-Location
param(
    [switch]$build,
    [switch]$prerelease,
    [string]$repoUrl = "",
    [string]$releaseName = "",
    [string]$tagName = ""
)

$doBuild = $build.IsPresent
$isPrerelease = $prerelease.IsPresent

if ($repoUrl -eq "") {
    Write-Error "Missing required parameter: repoUrl"
    exit 1
}
if ($releaseName -eq "") {
    Write-Error "Missing required parameter: releaseName"
    exit 1
}
if ($tagName -eq "") {
    Write-Error "Missing required parameter: tagName"
    exit 1
}

echo "build: $doBuild, prerelease: $isPrerelease, repoUrl: $repoUrl, releaseName: '$releaseName', tagName: '$tagName'"
exit 1

### variables
$csproj = ".\App.csproj"
$sln = "..\..\LHQ_vs2022.sln"

$buildConfiguration = "Debug"

$packId = "ScaleHQSolutions.LhqEditorApp"
$packTitle = "LHQ Editor App"
$packDir = ".\_published"
$mainExe = "LHQ.App.exe"
$icon = ".\Icon.ico"
$outputDir = ".\Releases"

### read version from csproj
$xml = [Xml] (Get-Content $csproj)
$propertyGroup = $xml.Project.PropertyGroup | Where-Object { $_.Version -ne $null }

if ($propertyGroup -eq $null) {
    Write-Error "Missing <Version> within <PropertyGroup> element in '$csproj' file!"
    exit 1
}
$packVersion = $propertyGroup.Version

# vpk download from github releases
$preArg = $isPrerelease ? "--pre" : ""
vpk download github --repoUrl $repoUrl --outputDir $outputDir --timeout 5 $preArg

### build solution first
if ($doBuild) {
    echo "Restoring nuget packages"
    dotnet restore $sln
    echo "Building solution $sln"

    echo "Publishing $packId (version $packVersion) to $packDir"
    dotnet publish -c $buildConfiguration -r win-x64 --no-self-contained -o $packDir

    if ($LASTEXITCODE -ne 0) {
        Write-Error "dotnet publish failed. Stopping script."
        exit 1
    }
}

### pack with velopack
# later test: --framework net8.0.204-x64-runtime
# --framework net8.0-x64-desktop
$framework = "net48"
echo "Packing $packId (version $packVersion, framework: $framework) to $outputDir"
vpk --yes pack --packId $packId --packVersion $packVersion `
    --packDir $packDir --mainExe $mainExe --packTitle $packTitle `
    --icon $icon --outputDir $outputDir --framework $framework --shortcuts None --noPortable

### vpk upload to github releases
echo "Uploading package to GitHub Releases"
vpk upload github --repoUrl $githubRepo --releaseName $releaseName --tag $tagName --merge $preArg
param(
    [switch]$build,
    [switch]$prerelease,
    [string]$repoUrl = "",
    [string]$releaseName = "",
    [string]$tagName = "",
    [string]$newVersion = "",
    [string]$workingDir = ""
)

if ($workingDir -ne "") {
    Set-Location $workingDir
}

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
if ($newVersion -eq "") {
    Write-Error "Missing required parameter: newVersion"
    exit 1
}

echo "build: $doBuild, prerelease: $isPrerelease, repoUrl: $repoUrl, releaseName: '$releaseName', tagName: '$tagName', newVersion: '$newVersion'"
#exit 1

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
$packVersion = $newVersion

### read version from csproj
$appProjectXml = [Xml] (Get-Content $csproj)
$appProjectPropGroup = $appProjectXml.Project.PropertyGroup | Where-Object { $_.Version -ne $null }

if ($appProjectPropGroup -eq $null) {
    Write-Error "Missing <Version> within <PropertyGroup> element in '$csproj' file!"
    exit 1
}
### update App.csproj with new version
$appProjectPropGroup.Version = $packVersion
$appProjectXml.Save((Resolve-Path $csproj))
echo "Updated App.csproj with version $packVersion"

#$packVersion = $appProjectPropGroup.Version

# vpk download from github releases
$preArg = $isPrerelease ? "--pre" : ""

try
{
    vpk download github --repoUrl $repoUrl --outputDir $outputDir --timeout 5 $preArg
}
catch {
    Write-Host "Error: $($_.Exception.Message)"
    Write-Host "Update server not reachable or returned error. Skipping download of current version."
}

### update ProductAssemblyInfo.cs with new version
$productAssemblyInfoPath = "..\..\ProductAssemblyInfo.cs"
$assemblyInfoContent = Get-Content $productAssemblyInfoPath -Raw
$assemblyInfoContent = $assemblyInfoContent -replace '\[assembly: AssemblyVersion\("[^"]+"\)\]', "[assembly: AssemblyVersion(`"$packVersion`")]"
Set-Content -Path $productAssemblyInfoPath -Value $assemblyInfoContent -NoNewline
echo "Updated ProductAssemblyInfo.cs with version $packVersion"

#$assemblyInfoContent2 = Get-Content $productAssemblyInfoPath -Raw
#echo "ProductAssemblyInfo.cs: \n$assemblyInfoContent2" 


### build solution first
if ($doBuild)
{
#    echo "Restoring nuget packages"
#    dotnet restore $sln
#    echo "Building solution $sln"
#
#    msbuild $sln /p:Configuration=$buildConfiguration /v:minimal
#    
#    if ($LASTEXITCODE -ne 0) {
#        Write-Error "msbuild failed. Stopping script."
#        exit 1
#    }
}

echo "Restoring nuget packages"
dotnet restore

echo "Publishing $packId (version $packVersion) to $packDir"
dotnet publish -c $buildConfiguration -r win-x64 --no-self-contained -o $packDir

if ($LASTEXITCODE -ne 0) {
    Write-Error "dotnet publish failed. Stopping script."
    exit 1
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
echo "Uploading package to GitHub Releases, url: $githubRepo, release: '$releaseName', tag: $tagName, $preArg"
vpk upload github --repoUrl $githubRepo --releaseName $releaseName --tag $tagName --merge $preArg
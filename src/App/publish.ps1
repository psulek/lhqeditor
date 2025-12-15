### variables
# read <Version> value from StartMeUp.csproj
$csproj = ".\App.csproj"
$sln = "..\..\LHQ_vs2022.sln"

$buildConfiguration = "Debug"
$doBuild = $true
#$doBuild = $false

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

### build solution first
if ($doBuild)
{
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
    --icon $icon --outputDir $outputDir --framework $framework --shortcuts None

$githubRepo="https://github.com/psulek/lhqeditor"
#$releaseName="Version 2025.4-rc.1"
#$tagName="v2025.4-rc.1"
$releaseName="$packVersion"
$tagName=""
$token="..."

vpk upload github --repoUrl $githubRepo --releaseName $releaseName --merge --pre --token $token
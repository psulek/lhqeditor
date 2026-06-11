[CmdletBinding()]
param(
    [string]$NewVersion,
    [string]$AppProjectPath = "src/App/",
    [string]$RootPath = "."
)

$ErrorActionPreference = "Stop"

$resolvedRootPath = [System.IO.Path]::GetFullPath($RootPath)
$resolvedAppProjectPath = [System.IO.Path]::GetFullPath((Join-Path $resolvedRootPath $AppProjectPath))
$csproj = Join-Path $resolvedAppProjectPath "App.csproj"

if (-not (Test-Path $csproj)) {
    Write-Error "App.csproj file not found at path '$csproj'!"
    exit 1
}

### read version from csproj
$appProjectXml = [Xml](Get-Content $csproj)
$appProjectPropGroup = $appProjectXml.Project.PropertyGroup | Where-Object { $_.Version -ne $null }

if ($appProjectPropGroup -eq $null) {
    Write-Error "Missing <Version> within <PropertyGroup> element in '$csproj' file!"
    exit 1
}

$currentVersion = [string]$appProjectPropGroup.Version
if ([string]::IsNullOrWhiteSpace($currentVersion)) {
    Write-Error "Current version in '$csproj' is empty."
    exit 1
}

if ([string]::IsNullOrWhiteSpace($NewVersion)) {
    Write-Host "Current version in App.csproj: $currentVersion"
    $inputVersion = Read-Host "Enter new version (format: X.Y.Z, e.g. $currentVersion). Press Enter to use default [$currentVersion]"
    if ([string]::IsNullOrWhiteSpace($inputVersion)) {
        $NewVersion = $currentVersion
    }
    else {
        $NewVersion = $inputVersion
    }
}

if ([string]::IsNullOrWhiteSpace($NewVersion)) {
    Write-Error "Error: version is required."
    exit 1
}

if ($NewVersion -notmatch '^\d+\.\d+\.\d+$') {
    Write-Error "Error: invalid version '$NewVersion'. Expected format: X.Y.Z (three numbers separated by dots), e.g. $currentVersion"
    exit 1
}

### update App.csproj with new version
$appProjectPropGroup.Version = $NewVersion
$appProjectXml.Save((Resolve-Path $csproj))
echo "Updated App.csproj with version $NewVersion"

### update ProductAssemblyInfo.cs with new version
$productAssemblyInfoPath = Join-Path $resolvedRootPath "ProductAssemblyInfo.cs"
$assemblyInfoContent = Get-Content $productAssemblyInfoPath -Raw
$assemblyInfoContent = $assemblyInfoContent -replace '\[assembly: AssemblyVersion\("[^"]+"\)\]', "[assembly: AssemblyVersion(`"$NewVersion`")]"
Set-Content -Path $productAssemblyInfoPath -Value $assemblyInfoContent -NoNewline
echo "Updated ProductAssemblyInfo.cs with version $NewVersion"

### update VsExtension2022 AssemblyInfo.cs with new version
$vsExtAssemblyInfoPath = Join-Path $resolvedRootPath "src/VsExtension2022/Properties/AssemblyInfo.cs"
$vsExtAssemblyInfoContent = Get-Content $vsExtAssemblyInfoPath -Raw
$vsExtAssemblyInfoContent = $vsExtAssemblyInfoContent -replace '\[assembly: AssemblyVersion\("[^"]+"\)\]', "[assembly: AssemblyVersion(`"$NewVersion`")]"
Set-Content -Path $vsExtAssemblyInfoPath -Value $vsExtAssemblyInfoContent -NoNewline
echo "Updated VsExtension2022 AssemblyInfo.cs with version $NewVersion"

### update VsExtensionConstants.cs with new version
$vsExtConstantsPath = Join-Path $resolvedRootPath "src/VsExtension2022/VsExtensionConstants.cs"
$vsExtConstantsContent = Get-Content $vsExtConstantsPath -Raw
$vsExtConstantsContent = $vsExtConstantsContent -replace 'public const string Version = "[^"]+";', "public const string Version = `"$NewVersion`";"
Set-Content -Path $vsExtConstantsPath -Value $vsExtConstantsContent -NoNewline
echo "Updated VsExtensionConstants.cs with version $NewVersion"

### update source.extension.vsixmanifest with new version
$manifestPath = Join-Path $resolvedRootPath "src/VsExtension2022/source.extension.vsixmanifest"
[xml]$manifestXml = Get-Content $manifestPath
$nsManager = New-Object System.Xml.XmlNamespaceManager($manifestXml.NameTable)
$nsManager.AddNamespace("vs", "http://schemas.microsoft.com/developer/vsx-schema/2011")
$identityNode = $manifestXml.SelectSingleNode("//vs:PackageManifest/vs:Metadata/vs:Identity", $nsManager)
if ($identityNode -ne $null) {
    $identityNode.Version = $NewVersion
    $manifestXml.Save((Resolve-Path $manifestPath))
    echo "Updated source.extension.vsixmanifest with version $NewVersion"
} else {
    Write-Error "Identity node not found in '$manifestPath'!"
    exit 1
}


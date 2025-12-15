$registry = "https://npm.pkg.github.com/"
$packageName = "@psulek/lhq-generators"
#$packageVersion = "1.0.64"
$packageVersion = "latest"

echo $PSScriptRoot
# Change the working directory to the script's directory
Set-Location $PSScriptRoot

#pnpm add $packageName@$packageVersion --registry $registry -P
# pnpm add $packageName@$packageVersion -P    

# pnpm install --offline
#pnpm install
$authToken = $env:GH_READ_PACKAGES_TOKEN
if (-not $authToken) {
    Write-Error "Environment variable 'GH_READ_PACKAGES_TOKEN' is not set. Please set it to your GitHub Packages read token."
    exit 1
}

#$env:NPM_TOKEN=$authToken
pnpm install

$packageJson = Join-Path $PSScriptRoot "package.json"
$node_modules = Join-Path $PSScriptRoot "node_modules"
$lib_folder = Join-Path $node_modules "@psulek\lhq-generators\"
$out = Join-Path $PSScriptRoot "content"
$out_browser = Join-Path $out "browser"
$out_templates = Join-Path $out "hbs"

if (Test-Path -Path $out) {
    Remove-Item -Path $out -Recurse -Force
}

if (!(Test-Path -Path $out)) {
    New-Item -ItemType Directory -Path $out -Force | Out-Null
}

if (!(Test-Path -Path $out_browser)) {
    New-Item -ItemType Directory -Path $out_browser -Force | Out-Null
}

if (!(Test-Path -Path $out_templates)) {
    New-Item -ItemType Directory -Path $out_templates -Force | Out-Null
}

Copy-Item -Recurse -Force $lib_folder\browser\index.js $out_browser
Copy-Item -Recurse -Force $lib_folder\hbs\*.* $out_templates

# Remove-Item -Path $node_modules -Recurse -Force
# Remove-Item -Path $packageJson -Recurse -Force
$registry = "http://localhost:4873/"
$packageName = "@lhq/lhq-generators"
#$packageVersion = "1.0.64"
$packageVersion = "latest"

echo $PSScriptRoot

pnpm add $packageName@$packageVersion --registry $registry -P
pnpm install --offline

$packageJson = Join-Path $PSScriptRoot "package.json"
$node_modules = Join-Path $PSScriptRoot "node_modules"
$lib_folder = Join-Path $node_modules "@lhq\lhq-generators\"
$out = Join-Path $PSScriptRoot "content"
$out_browser = Join-Path $out "browser"
$out_templates = Join-Path $out "templates"

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

Copy-Item -Recurse -Force $lib_folder\browser\*.* $out_browser
Copy-Item -Recurse -Force $lib_folder\templates\*.* $out_templates

Remove-Item -Path $node_modules -Recurse -Force
Remove-Item -Path $packageJson -Recurse -Force
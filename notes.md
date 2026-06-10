To manually build & create portable release build, without auto-updates features, run the following commands:

```bash
dotnet publish -c Debug -r win-x64 --no-self-contained -o ".\_published" -p:DisableCheckForUpdates=True
```

```bash
$packId = "ScaleHQSolutions.LhqEditorApp"
$packTitle = "LHQ Editor App"
$packDir = ".\_published"
$mainExe = "LHQ.App.exe"
$icon = ".\Icon.ico"
$outputDir = ".\Releases"
$packVersion = "2025.3.3"
$framework = "net48"

vpk --yes pack --packId $packId --packVersion $packVersion     --packDir $packDir --mainExe $mainExe --packTitle $packTitle     --icon $icon --outputDir $outputDir --framework $framework --shortcuts None
```

Where: 
 `-p:DisableCheckForUpdates=True` - disables auto-updates features, so the app will not check for updates for selected portable build, like version 2025.3.3 with model version v2.
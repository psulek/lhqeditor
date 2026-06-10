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
$packVersion = (Select-Xml -Path .\App.csproj -XPath '/Project/PropertyGroup/Version').Node.'#text' | Select-Object -First 1
$framework = "net48"



vpk --yes pack --packId $packId --packVersion $packVersion     --packDir $packDir --mainExe $mainExe --packTitle $packTitle     --icon $icon --outputDir $outputDir --framework $framework --shortcuts None
```

Where: 
 `-p:DisableCheckForUpdates=True` - disables auto-updates features, so the app will not check for updates for selected portable build, like version 2025.3.3 with model version v2.


To test session 0 installation of the app, you can use the following command:

```bash
  $fullExePath = (Resolve-Path "$outputDir\ScaleHQSolutions.LhqEditorApp-win-Setup.exe").Path
  $fullExeArgs = "--installto C:\LHQEditorApp --silent"
  $taskAction = New-ScheduledTaskAction -Execute $fullExePath -Argument $fullExeArgs
  $principal = New-ScheduledTaskPrincipal -UserId "SYSTEM" -LogonType ServiceAccount -RunLevel Highest
  Register-ScheduledTask -TaskName "TestLHQEditorAppInstall" -Action $taskAction -Principal $principal
  Start-ScheduledTask -TaskName "TestLHQEditorAppInstall"
```

  Then check the registry to confirm entries went to HKLM only. Clean up with:
```bash
  Unregister-ScheduledTask -TaskName "TestLHQEditorAppInstall" -Confirm:$false
```

To test session 0 uninstall of app, you can use the following command:

```bash
  $taskName = "TestLHQEditorAppUninstall"
  $taskAction = New-ScheduledTaskAction -Execute "C:\LHQEditorApp\Update.exe" -Argument "--uninstall --silent"
  $principal = New-ScheduledTaskPrincipal -UserId "SYSTEM" -LogonType ServiceAccount -RunLevel Highest
  Register-ScheduledTask -TaskName $taskName -Action $taskAction -Principal $principal
  Start-ScheduledTask -TaskName "TestLHQEditorAppUninstall"
```

Then check the registry to confirm entries were removed from HKLM. Clean up with:
```bash
  Unregister-ScheduledTask -TaskName "TestLHQEditorAppUninstall" -Confirm:$false
```
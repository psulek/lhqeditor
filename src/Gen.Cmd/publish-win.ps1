if (Test-Path './_published') { Remove-Item -Recurse -Force './_published' }
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:EnableCompressionInSingleFile=true -p:IncludeAllContentForSelfExtract=true -o ./_published/win

Get-ChildItem -Path './_published' -Recurse -Filter '*.pdb' | Remove-Item -Force

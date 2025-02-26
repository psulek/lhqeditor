if (Test-Path '../../_published') { Remove-Item -Recurse -Force '../../_published' }
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:EnableCompressionInSingleFile=true -p:IncludeAllContentForSelfExtract=true -o ../../_published/win

$pdbFile = Resolve-Path (Join-Path -Path "../../_published/win" -ChildPath "LHQ.Gen.Lib.pdb")

if (Test-Path $pdbFile) {
    Remove-Item $pdbFile -Force
}
# Start-Job -ScriptBlock { verdaccio }


$processId = Get-WmiObject Win32_Process | Where-Object { $_.CommandLine -match "node_modules/verdaccio/bin/verdaccio" } | Select-Object ProcessId

if ($processId) {
    echo "Stopping existing verdaccio processes..."
    $processId | ForEach-Object { Stop-Process -Id $_.ProcessId -Force }
}

echo "Starting new verdaccio process..."
# start verdaccio using powershell as background process (detached from running this script)
Start-Process -FilePath "powershell.exe" -ArgumentList "-File", "`"C:\Users\peter.sulek\AppData\Local\pnpm\verdaccio.ps1`"" -WindowStyle Hidden

# get process id of powershell with runs verdaccio
Get-WmiObject Win32_Process | Where-Object { $_.CommandLine -match "node_modules/verdaccio/bin/verdaccio" } | Select-Object CommandLine, ProcessId

Start-Process 'http://localhost:4873/'
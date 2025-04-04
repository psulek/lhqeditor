# get process id of verdaccio if it is running
$processId = Get-WmiObject Win32_Process | Where-Object { $_.CommandLine -match "node_modules/verdaccio/bin/verdaccio" } | Select-Object ProcessId

# when verdaccio is running, stop it (supports multiple processes)
if ($processId) {
    echo "Stopping existing verdaccio processes..."
    $processId | ForEach-Object { 
        echo "Stopping process with ID: $($_.ProcessId)"
        Stop-Process -Id $_.ProcessId -Force 
    }
}

# start verdaccio using powershell as background process (detached from running this script)
echo "Starting new verdaccio process..."
Start-Process -FilePath "powershell.exe" -ArgumentList "-File", "`"C:\Users\peter.sulek\AppData\Local\pnpm\verdaccio.ps1`"" -WindowStyle Hidden

# wait for verdaccio to start before opening the browser
Start-Sleep -Seconds 1

# open verdaccio in the default browser
Start-Process 'http://localhost:4873/'
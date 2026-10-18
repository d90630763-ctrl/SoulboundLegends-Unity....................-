param([string]$ConnectionString,[string]$OutputFile="soulbound_$(Get-Date -Format yyyyMMdd_HHmmss).dump")
if([string]::IsNullOrWhiteSpace($ConnectionString)){throw "ConnectionString is required."}
pg_dump --format=custom --no-owner --file=$OutputFile $ConnectionString
if($LASTEXITCODE -ne 0){throw "pg_dump failed."}

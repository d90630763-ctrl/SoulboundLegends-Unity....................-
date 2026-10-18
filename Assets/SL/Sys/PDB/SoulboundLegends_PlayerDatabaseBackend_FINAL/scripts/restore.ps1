param([string]$ConnectionString,[Parameter(Mandatory=$true)][string]$BackupFile)
if(!(Test-Path $BackupFile)){throw "Backup file not found."}
pg_restore --clean --if-exists --no-owner --dbname=$ConnectionString $BackupFile
if($LASTEXITCODE -ne 0){throw "pg_restore failed."}

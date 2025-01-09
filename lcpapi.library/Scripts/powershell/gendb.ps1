# Set project paths
$pthproj = "$env:USERPROFILE\Documents\projects\lcpapi\lcpapi"
$pthmig = "$pthproj\Migrations"
$DefDBMode = "SQLite"

# Change directory to project path
Set-Location -Path $pthproj

# Main function
function Main {
    cls
    Write-Host ""
    Write-Host "Generate DB for API"
    Write-Host ""
    Write-Host "--------------------------------------"
    Write-Host "Author Info:"
    Write-Host "Name: Luis Carvalho"
    Write-Host "Email: luiscarvalho239@gmail.com"
    Write-Host "Date creation of script: 30/09/2024"
    Write-Host "--------------------------------------"
    Write-Host ""
    Write-Host "Choose your option:"
    Write-Host ""
    Write-Host "Note: The default option is $DefDBMode"
    Write-Host ""
    Write-Host "1 - SQLite"
    Write-Host "2 - SQLServer"
    Write-Host "3 - PostgresSQL"
    Write-Host "4 - MySQL"
    Write-Host "5 - All"
    Write-Host ""
    $chdbmode = Read-Host "Enter your choice"

    switch ($chdbmode) {
        "" { AddSQLite }
        "1" { AddSQLite }
        "2" { AddSQLServer }
        "3" { AddPostgresSQL }
        "4" { AddMySQL }
        "5" { AddAll }
        default { InvalidChoice }
    }
}

# Check if dotnet ef is installed
function CheckDotnetEFInstalled {
    dotnet ef > $null 2>&1
    if ($LASTEXITCODE -ne 0) {
        dotnet tool install --global dotnet-ef
    } else {
        Write-Host "Dotnet EF Core Tools installed"
    }
}

# Add database
function AddDB {
    param (
        [string]$dbMode
    )
    CheckDotnetEFInstalled

    if (Test-Path "$pthmig\$dbMode") {
        Remove-Item -Recurse -Force "$pthmig\$dbMode"
    }

    if (Test-Path "$pthproj\Database\$dbMode") {
        Remove-Item -Recurse -Force "$pthproj\Database\$dbMode"
    }

    if (-not (Test-Path "$pthproj\Database\$dbMode")) {
        New-Item -ItemType Directory -Path "$pthproj\Database\$dbMode"
    }

    dotnet ef migrations remove --force --context "MyDBContext$dbMode"
    dotnet ef database drop --force --context "MyDBContext$dbMode"
    dotnet ef migrations add "InitialCreate$dbMode" --context "MyDBContext$dbMode" --output-dir "$pthmig\$dbMode"
    dotnet ef database update "InitialCreate$dbMode" --context "MyDBContext$dbMode"
    dotnet ef migrations script --context "MyDBContext$dbMode" --output "Scripts/sql/migscr$dbMode.sql"
}

# Add SQLite database
function AddSQLite {
    cls
    $DefDBMode = "SQLite"
    Write-Host $DefDBMode
    AddDB -dbMode $DefDBMode
}

# Add SQL Server database
function AddSQLServer {
    cls
    $DefDBMode = "SQLServer"
    Write-Host $DefDBMode
    AddDB -dbMode $DefDBMode
}

# Add PostgreSQL database
function AddPostgresSQL {
    cls
    $DefDBMode = "PostgresSQL"
    Write-Host $DefDBMode
    AddDB -dbMode $DefDBMode
}

# Add MySQL database
function AddMySQL {
    cls
    $DefDBMode = "MySQL"
    Write-Host $DefDBMode
    AddDB -dbMode $DefDBMode
}

# Add all databases
function AddAll {
    AddSQLite
    AddSQLServer
    AddPostgresSQL
    AddMySQL
}

# Invalid choice
function InvalidChoice {
    cls
    Write-Host "Invalid choice!"
}

# Run main function
Main

# Pause before exit
Pause

param()

$projectPath = Join-Path $PSScriptRoot "lcpapi.unittests.csproj"

Write-Host "Building the project before running tests..."
dotnet clean $projectPath
dotnet build $projectPath

Write-Host "Running tests for lcpapi.unittests..."
Write-Host "Project: $projectPath"
$testResult = dotnet test $projectPath

if ($LASTEXITCODE -ne 0) {
    Write-Error "dotnet test failed with exit code $LASTEXITCODE"
    exit $LASTEXITCODE
}

Write-Host "Tests completed successfully."
write-Host "Test results:"
write-Host $testResult
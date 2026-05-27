param()

$projectPath = Join-Path $PSScriptRoot "lcpapi.unittests.csproj"
$settingsPath = Join-Path $PSScriptRoot "coverage.runsettings"
$resultsDir = Join-Path $PSScriptRoot "TestResults"
$coverageReportDir = Join-Path $resultsDir "CoverageReport"

Write-Host "Running tests with coverage for lcpapi.unittests..."
Write-Host "Project: $projectPath"
Write-Host "Settings: $settingsPath"
Write-Host "Results: $resultsDir"
Write-Host "Coverage report: $coverageReportDir"

$testResult = dotnet test $projectPath --settings $settingsPath --results-directory $resultsDir
if ($LASTEXITCODE -ne 0) {
    Write-Error "dotnet test failed with exit code $LASTEXITCODE"
    exit $LASTEXITCODE
}

if (-not (Get-Command reportgenerator -ErrorAction SilentlyContinue)) {
    Write-Warning "ReportGenerator is not installed. Install it with: dotnet tool install --global dotnet-reportgenerator-globaltool"
    Write-Host "Coverage files remain available under: $resultsDir"
    exit 0
}

$coverageFile = Get-ChildItem -Path "$resultsDir\*\coverage.opencover.xml" | Sort-Object LastWriteTime -Descending | Select-Object -First 1
if (-not $coverageFile) {
    Write-Error "Could not find coverage.opencover.xml in $resultsDir"
    exit 1
}

Write-Host "Generating coverage reports from: $($coverageFile.FullName)"
reportgenerator -reports:"$($coverageFile.FullName)" -targetdir:"$coverageReportDir" -reporttypes:"Html;JsonSummary"

if ($LASTEXITCODE -ne 0) {
    Write-Error "Report generation failed with exit code $LASTEXITCODE"
    exit $LASTEXITCODE
}

Write-Host "Coverage report generation complete. Open: $coverageReportDir\index.html"

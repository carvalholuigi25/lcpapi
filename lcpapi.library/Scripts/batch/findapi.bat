@echo off
setlocal enabledelayedexpansion

set "SEARCH_ROOT=%cd%\..\..\..\"
set "FOLDER_NAME=lcpapi"

for /d /r "%SEARCH_ROOT%" %%G in (*) do (
    if /i "%%~nxG"=="%FOLDER_NAME%" (
        set "RESULT=%%G"
        goto :found
    )
)

echo No folder named "%FOLDER_NAME%" found.
endlocal
pause
exit /b 1

:found
echo Found: %RESULT%
:: Use %RESULT% for whatever you need here
endlocal
pause
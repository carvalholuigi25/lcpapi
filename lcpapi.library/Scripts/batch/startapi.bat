@echo off
setlocal enableextensions

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
SET "pthproj=%RESULT%"
call :main
pause
exit /b 0

:main
cd %pthproj%

REM netstat -ano | findstr :5000
REM taskkill /PID <process_id> /F

FOR /F "tokens=5 delims= " %%A IN ('netstat -ano ^| findstr :5000') DO (
    taskkill /PID %%A /F
)

taskkill /f /im dotnet.exe
dotnet watch

if %errorlevel% neq 0 ( 
   echo LCPAPI is now starting...
) else (
   echo LCPAPI couldnt be started...
)

goto :end

:end
pause
exit

endlocal
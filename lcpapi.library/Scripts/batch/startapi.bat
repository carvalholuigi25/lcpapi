@echo off
setlocal enableextensions

@REM set "SEARCH_ROOT=%cd%\..\..\..\"
@REM set "SEARCH_ROOT=%%~dp0\..\..\..\"
@REM set "SEARCH_ROOT=%%~dp0\..\..\..\..\..\"
SET "SEARCH_ROOT=%userprofile%\mydocs"
set "FOLDER_NAME=lcpapi"

for /d /r "%SEARCH_ROOT%" %%G in (*) do (
    if /i "%%~nxG"=="%FOLDER_NAME%" (
        set "RESULT=%%G"
        goto :found
    )
)

echo No folder named "%SEARCH_ROOT%\%FOLDER_NAME%" found.
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
cd %pthproj%/%FOLDER_NAME%

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
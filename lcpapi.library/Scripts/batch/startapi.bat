@echo off
setlocal enableextensions
SET "pthproj=%userprofile%\Documents\projects\lcpapi\lcpapi"

call :main

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
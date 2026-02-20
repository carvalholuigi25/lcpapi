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

if %errorlevel% neq 0 ( 
   echo LCPAPI is now stopped...
) else (
   echo LCPAPI couldnt be stopped...
)

goto :end

:end
pause
exit

endlocal
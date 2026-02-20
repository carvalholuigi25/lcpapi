@echo off
setlocal enableextensions
SET "pthproj=%userprofile%\Documents\projects\lcpapi\lcpapi.library"
REM for /f "delims=" %%A in ('hostname') do set "mname=%%A"

cd %pthproj%

call :main

:startAPI
cls
call %pthproj%\Scripts\batch\startapi.bat
REM runas /noprofile /user:"%mname%\luisc" "%pthproj%\Scripts\batch\startapi.bat"
exit 0

:stopAPI
cls
call %pthproj%\Scripts\batch\stopapi.bat
REM runas /noprofile /user:"%mname%\luisc" "%pthproj%\Scripts\batch\stopapi.bat"
exit 0

:genDB
cls
call %pthproj%\Scripts\batch\gendb.bat
exit 0

:main
cls
echo.
echo LCPAPI Main Menu
echo.
echo --------------------------------------
echo Author Info:
echo Name: Luis Carvalho
echo Email: luiscarvalho239@gmail.com
echo Date creation of script: 20/10/2025
echo --------------------------------------
echo.
echo Choose your option:
echo.
echo A - Start API
echo B - Stop API
echo C - Generate DB
echo.
set /p chmmode=""

if "%chmmode%" EQU "" ( goto :startAPI )
if "%chmmode%" EQU "A" ( goto :startAPI )
if "%chmmode%" EQU "a" ( goto :startAPI )
if "%chmmode%" EQU "B" ( goto :stopAPI )
if "%chmmode%" EQU "b" ( goto :stopAPI )
if "%chmmode%" EQU "C" ( goto :genDB )
if "%chmmode%" EQU "c" ( goto :genDB )
goto :invChoice

:invChoice
cls
echo Invalid choice!
goto :end

:end
pause
exit

:endalt
exit /b 0

endlocal

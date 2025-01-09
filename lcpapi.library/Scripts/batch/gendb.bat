@echo off
setlocal enableextensions
SET "pthproj=%userprofile%\Documents\projects\lcpapi\lcpapi"
SET "pthmig=%pthproj%\Migrations"
SET DefDBMode=SQLite

cd %pthproj%

call :main

:chkDNEFInstalled
dotnet ef >nul 2>&1
if %errorlevel% neq 0 (
    dotnet tool install --global dotnet-ef
) else (
    echo Dotnet EF Core Tools installed
)
goto :endalt

:addDB
call :chkDNEFInstalled

if exist "%pthmig%\%~1" (
    rmdir /s /q "%pthmig%\%~1"
)

if exist "%pthproj%\Database\%~1" (
    rmdir /s /q "%pthproj%\Database\%~1"
)

if not exist "%pthproj%\Database\%~1" (
    mkdir "%pthproj%\Database\%~1"
)

dotnet ef migrations remove --force --context "MyDBContext%~1"
dotnet ef database drop --force --context "MyDBContext%~1"
dotnet ef migrations add "InitialCreate%~1" --context "MyDBContext%~1" --output-dir "%pthmig%/%~1"
dotnet ef database update "InitialCreate%~1" --context "MyDBContext%~1"
dotnet ef migrations script --context "MyDBContext%~1" --output "Scripts/sql/migscr%~1.sql"
goto :endalt

:addSQLite
cls
SET DefDBMode=SQLite
echo %DefDBMode%
call :addDB %DefDBMode%
goto :endalt

:addSQLServer
cls
SET DefDBMode=SQLServer
echo %DefDBMode%
call :addDB %DefDBMode%
goto :endalt

:addPostgresSQL
cls
SET DefDBMode=PostgresSQL
echo %DefDBMode%
call :addDB %DefDBMode%
goto :endalt

:addMySQL
cls
SET DefDBMode=MySQL
echo %DefDBMode%
call :addDB %DefDBMode%
goto :endalt

:addAll
call :addSQLite
call :addSQLServer
call :addPostgresSQL
call :addMySQL
goto :end

:main
cls
echo.
echo Generate DB for API
echo.
echo --------------------------------------
echo Author Info:
echo Name: Luis Carvalho
echo Email: luiscarvalho239@gmail.com
echo Date creation of script: 30/09/2024
echo --------------------------------------
echo.
echo Choose your option:
echo.
echo Note: The default option is %DefDBMode%
echo.
echo 1 - SQLite
echo 2 - SQLServer
echo 3 - PostgresSQL
echo 4 - MySQL
echo 5 - All
echo.
set /p chdbmode=""

if "%chdbmode%" EQU "" ( goto :addSQLite )
if "%chdbmode%" EQU "1" ( goto :addSQLite )
if "%chdbmode%" EQU "2" ( goto :addSQLServer )
if "%chdbmode%" EQU "3" ( goto :addPostgresSQL )
if "%chdbmode%" EQU "4" ( goto :addMySQL )
if "%chdbmode%" EQU "5" ( goto :addAll )
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

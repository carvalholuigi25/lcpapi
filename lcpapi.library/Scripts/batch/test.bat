@echo off
FOR /F "tokens=5 delims= " %%A IN ('netstat -ano ^| findstr :5000') DO (
    echo PID: %%A
)
pause
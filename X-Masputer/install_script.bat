@echo off
set TASKNAME=X-MasputerAutostart
set PARAMETERS=--autostart

REM Get the absolute path of the script directory
set SCRIPT_PATH=%~dp0
set EXEPATH=%SCRIPT_PATH%X-Masputer.exe

REM Create the scheduled task with highest privileges
schtasks /create /tn "%TASKNAME%" /tr "\"%EXEPATH%\" %PARAMETERS%" /sc onlogon /ru "%USERNAME%" /RL HIGHEST /f

if %errorlevel%==0 (
    echo Task '%TASKNAME%' was successfully created.
) else (
    echo Failed to create the task '%TASKNAME%'.
)
pause

@echo off
set TASKNAME=X-MasputerAutostart

REM Delete the scheduled task
schtasks /delete /tn "%TASKNAME%" /f

if %errorlevel%==0 (
    echo Task '%TASKNAME%' was successfully deleted.
) else (
    echo Failed to delete the task '%TASKNAME%'. It may not exist.
)
pause

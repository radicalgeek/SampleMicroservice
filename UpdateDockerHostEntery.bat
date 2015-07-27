@ECHO OFF
ECHO Getting boot2docker ip...
FOR /f "usebackq tokens=*" %%a IN (`boot2docker ip`) DO SET localdocker-ip=%%a
ECHO %localdocker-ip%
ECHO.
ECHO Analyzing hosts file
%windir%\System32\FIND /C /I "rook-queue" %windir%\System32\drivers\etc\hosts
IF %ERRORLEVEL% NEQ 0 (
    ECHO rook-queue not found, adding to hosts...
    powershell -command  "%~dp0Add-Hosts.ps1 -hostName \"rook-queue\" -hostIp \"%localdocker-ip%\""
) ELSE (
    ECHO rook-queue found, updating hosts...
    powershell -command "%~dp0Update-Hosts.ps1 -hostName \"rook-queue\" -hostIp \"%localdocker-ip%\""
)
%windir%\System32\FIND /C /I "rook-sample-db" %windir%\System32\drivers\etc\hosts
IF %ERRORLEVEL% NEQ 0 (
    ECHO rook-sample-db not found, adding to hosts...
    powershell -command "%~dp0Add-Hosts.ps1 -hostName \"rook-sample-db\" -hostIp \"%localdocker-ip%\""
) ELSE (
    ECHO rook-sample-db found, updating hosts...
    powershell -command "%~dp0Update-Hosts.ps1 -hostName \"rook-sample-db\" -hostIp \"%localdocker-ip%\""
)
ECHO.
pause
@ECHO ON

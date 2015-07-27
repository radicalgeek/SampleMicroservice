@ECHO OFF
ECHO Getting boot2docker ip...
FOR /f "usebackq tokens=*" %%a IN (`boot2docker ip`) DO SET localdocker-ip=%%a
ECHO %localdocker-ip%
ECHO.
ECHO Analyzing hosts file
%windir%\System32\FIND /C /I "localdocker" %windir%\System32\drivers\etc\hosts
IF %ERRORLEVEL% NEQ 0 (
    ECHO localdocker not found, adding to hosts...
    powershell -command "Add-Hosts.ps1 -hostName \"localdocker\" -hostIp \"%localdocker-ip%\""
) ELSE (
    ECHO localdocker found, updating hosts...
    powershell -command "Update-Hosts.ps1 -hostName \"localdocker\" -hostIp \"%localdocker-ip%\""
)
ECHO.

@ECHO ON

Param(
    [Parameter(Mandatory=$true)]
    [string]$hostName,
    [Parameter(Mandatory=$true)]
    [string]$hostIp
)

function IsAdministrator
{
    $Identity = [System.Security.Principal.WindowsIdentity]::GetCurrent()
    $Principal = New-Object System.Security.Principal.WindowsPrincipal($Identity)
    $Principal.IsInRole([System.Security.Principal.WindowsBuiltInRole]::Administrator)
}

# Check to see if we are currently running "as Administrator"
if (!(IsAdministrator))
{
   # We are not running "as Administrator" - so relaunch as administrator
   [string[]]$argList = @('-NoProfile', '-File', $MyInvocation.MyCommand.Path)
   $argList += $MyInvocation.BoundParameters.GetEnumerator() | Foreach {"-$($_.Key)", "$($_.Value)"}
   $argList += $MyInvocation.UnboundArguments
   Start-Process PowerShell.exe -Verb Runas -WorkingDirectory $pwd -ArgumentList $argList 
   return
}

write "updating hosts file.."

$hostsfile = "$env:windir\System32\drivers\etc\hosts"
(gc $hostsfile ) -replace "[0-9.]+ $hostName", "$hostIp $hostName" | sc $hostsfile

write "done"

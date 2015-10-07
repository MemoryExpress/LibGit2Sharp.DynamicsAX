SET DIR=%~dp0%
cd /d %DIR%
PowerShell.exe -NoExit -ExecutionPolicy Unrestricted -Command "& '%DIR%Install.ps1' %*"
PAUSE
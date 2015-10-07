$CompileDir = Join-Path $PSScriptRoot "LibGit2Sharp.DynamicsAX\bin\Debug"

Function Get-AOSBinDir()
{
    $key = "HKLM:\SYSTEM\CurrentControlSet\services\Dynamics Server\6.0\01\Original (installed configuration)"
    if(-not (Test-Path $key))
    {
        return "";
    }
    
    return (Get-ItemProperty -Path $key -Name bindir).bindir
}

Function Get-ClientBinDir()
{
    $key = "HKCU:\Software\Microsoft\Dynamics\6.0\Configuration\Original (installed configuration)"
    if(-not (Test-Path $key))
    {
        return "";
    }

    return (Get-ItemProperty -Path $key -Name bindir).bindir
}

Function Get-VSBinDir()
{
    $key = "HKCU:\Software\Microsoft\VisualStudio\12.0_Config"
    if(-not (Test-Path $key))
    {
        return "";
    }

    return (Get-ItemProperty -Path $key -Name InstallDir).InstallDir
}

Function Copy-To($to)
{
    $from = $CompileDir
    Get-ChildItem -Path $from | % { 
      Copy-Item $_.fullname "$to" -Recurse -Force #-Exclude @("app", "main.js") 
    }
}

$AOSBinDir = Get-AOSBinDir
$ClientBinDir = Get-ClientBinDir
$VSBinDir = Get-VSBinDir

if($AOSBinDir){
    Write-Host "Installing to $AOSBinDir"

    Copy-To $AOSBinDir
}

if($ClientBinDir){
    Write-Host "Installing to $ClientBinDir"

    Copy-To $ClientBinDir
}

if($VSBinDir){
    Write-Host "Installing to $VSBinDir"

    Copy-To $VSBinDir
}
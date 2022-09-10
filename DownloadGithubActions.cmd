@echo off
setlocal
if not exist "%~dp0.github\workflows" call mkdir "%~dp0.github\workflows"
if errorlevel 1 exit /B %ERRORLEVEL%
call powershell.exe -ExecutionPolicy Bypass -NoLogo -Command "& {$WebClient = New-Object System.Net.WebClient; $WebClient.DownloadFileTaskAsync('https://raw.githubusercontent.com/karashiiro/DalamudPluginProjectTemplate/master/.github/workflows/dotnet.yml', '%~dp0.github\workflows\dotnet.yml').Wait(); $WebClient.Dispose(); $WebClient = $null}"
if errorlevel 1 exit /B %ERRORLEVEL%

@echo off

dotnet tool list --global | findstr fake >NUL 2>&1
if errorlevel 1 dotnet tool install --global fake-cli

rem Pass -t <target> to run a target including dependencies.
rem Pass -s -t <target> to run a single target w/o dependencies.
fake build %*
if errorlevel 1 exit /b %errorlevel%

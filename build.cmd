@echo off

dotnet tool install --global fake-cli

rem Pass -t <target> to run a target including dependencies.
rem Pass -s -t <target> to run a single target w/o dependencies.
fake build %*
if errorlevel 1 exit /b %errorlevel%

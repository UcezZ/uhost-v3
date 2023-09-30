@echo off
set t=%cd%
cd /d "%~dp0..\..\.."
docker build -t ucezz/dotnet-aspnet-gdi:6.0 -f "%~dp0dockerfile" .
cd /d "%t%"
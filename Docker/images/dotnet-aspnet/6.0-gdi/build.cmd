@echo off
set t=%cd%
cd /d "%~dp0..\..\.."
docker build -t ucezz/dotnet-aspnet:6.0-gdi -f "%~dp0dockerfile" .
cd /d "%t%"
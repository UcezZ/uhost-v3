@echo off
set t=%cd%
cd /d "%~dp0..\..\.."
docker build -t ucezz/dotnet-aspnet:5.0-gdi-ffmpeg -f "%~dp0dockerfile" .
cd /d "%t%"
@echo off
set t=%cd%
cd /d "%~dp0..\.."
docker build -t ucezz/debian-gdi:stable-slim -f "%~dp0dockerfile" .
cd /d "%t%"
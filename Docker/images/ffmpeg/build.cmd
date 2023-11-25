@echo off
set t=%cd%
cd /d "%~dp0..\.."
docker build -t ucezz/ffmpeg:5.1.2 -f "%~dp0dockerfile" .
cd /d "%t%"
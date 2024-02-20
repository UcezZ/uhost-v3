@echo off
set t=%cd%
cd /d "%~dp0..\..\.."
docker build -t ucezz/node:18.19-rsync-alpine -f "%~dp0dockerfile" .
cd /d "%t%"
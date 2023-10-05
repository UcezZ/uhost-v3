@echo off
set t=%cd%
cd /d "%~dp0..\..\.."
docker build -t ucezz/node-rsync:16.17-alpine -f "%~dp0dockerfile" .
cd /d "%t%"
@echo off
set t=%cd%
cd /d "%~dp0"
docker build -t ucezz/nginx:1.25.5-alpine-3.18-vod-lua -f "%~dp0dockerfile" .
cd /d "%t%"
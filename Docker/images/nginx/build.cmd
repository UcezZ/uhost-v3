@echo off
set t=%cd%
cd /d "%~dp0"
docker build -t ucezz/nginx:1.16.1-alpine-3.12-vod-lua -f "%~dp0dockerfile" .
cd /d "%t%"
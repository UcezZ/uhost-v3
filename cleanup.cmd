@echo off
for /f "delims=" %%a in ('dir /b "%~dp0Uhost.*"') do for %%b in (bin,obj) do if exist "%~dp0%%~a\%%~b" (
    rd /q /s "%~dp0%%~a\%%~b"
    echo Removed %%~a\%%~b
)

if exist "%~dp0out" rd /q /s "%~dp0out"
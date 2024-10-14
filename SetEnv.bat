@echo off
setlocal

rem 获取当前脚本的目录
set "scriptDir=%~dp0"

rem 设置环境变量
set "PATH=%scriptDir%Util\bin\Release;%PATH%"

rem 可选：输出当前的 PATH 以确认设置
echo Current PATH: %PATH%

endlocal

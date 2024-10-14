@echo off
setlocal

rem 获取当前脚本的目录
set "scriptDir=%~dp0"

rem 定义 Excel 文件夹的绝对路径
set "excelFolder=%scriptDir%Excel"
set "csFolder=%scriptDir%Cs"
set "jsonFolder=%scriptDir%Json"

rem 执行 Util.exe，并传入 Excel 文件夹的路径
"%scriptDir%Util\bin\Release\net8.0\Util.exe" "%excelFolder%" "%csFolder%" "%jsonFolder%"

endlocal

pause
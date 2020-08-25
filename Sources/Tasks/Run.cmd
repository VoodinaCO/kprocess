@echo off
echo moving to KL2 SolutionDir

cd ..\..\..


set executable=".\KProcess.Ksmed.Presentation.Shell\bin\Debug\Ksmed2.exe"
set MSBuildDir="C:\Windows\Microsoft.NET\Framework64\v4.0.30319"


REM echo %executable%
REM pause
REM 
REM echo "checking executable compilation state"
REM if not exist %executable% (
REM 	echo KL not found. Trying to compile the solution...
REM 	@echo on
REM 	"%MSBuildDir%\msbuild.exe" ".\KProcess.Ksmed.sln"
REM 	@echo off
REM )

echo Launching application
cd ".\KProcess.Ksmed.Presentation.Shell\bin\Debug\"

echo Attaching VS...
echo Application should have been launch via CTRL+F5 (at least not with F5...)
Start /B Ksmed2.exe
FOR /F %%T IN ('Wmic process where^(Name^="Ksmed2.exe"^)get ProcessId^|more +1') DO (
SET /A ProcessId=%%T) &GOTO SkipLine                                                   
:SkipLine     
echo PID is %ProcessId%

echo debugger deactivated...
# vsjitDebugger -p %ProcessId%
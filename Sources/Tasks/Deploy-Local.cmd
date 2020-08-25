@IF [%1] == [] goto error
@IF [%2] == [] goto error
@IF [%3] == [] goto error
goto :deploy

:error
@echo Error: missing parameters
@echo Batch is now exiting
@pause
EXIT 1

:deploy
set SolutionDir=%1
set TargetDir=%2
set OutDir=%3

@echo %SolutionDir%
@echo "%TargetDir%KProcess.Ksmed.Ext.3D-Plus.dll"
@echo "%SolutionDir%KL2-Core\KProcess.Ksmed.Presentation.Shell\%OutDir%Extensions\3D-Plus"
@pause

REM set DeployTarget="%SolutionDir%..\..\KProcess.Ksmed.Presentation.Shell\%OutDir%Extensions\Dalkia"

mkdir %SolutionDir%KL2-Core\KProcess.Ksmed.Presentation.Shell\%OutDir%Extensions\0K-process
mkdir %SolutionDir%KL2-Core\KProcess.Ksmed.Presentation.Shell\%OutDir%Extensions\0K-process\Resources
mkdir %SolutionDir%KL2-Core\KProcess.Ksmed.Presentation.Shell\%OutDir%Extensions\0K-process\Video

xcopy /Y %TargetDir%System.Reactive.dll  %SolutionDir%KL2-Core\KProcess.Ksmed.Presentation.Shell\%OutDir%Extensions\0K-process
xcopy /Y %TargetDir%System.Reactive.Windows.Threading.dll  %SolutionDir%KL2-Core\KProcess.Ksmed.Presentation.Shell\%OutDir%Extensions\0K-process
xcopy /Y %TargetDir%KProcess.Ksmed.Ext.K-process.dll  %SolutionDir%KL2-Core\KProcess.Ksmed.Presentation.Shell\%OutDir%Extensions\0K-process
xcopy /Y %TargetDir%KProcess.Ksmed.Ext.K-process.pdb  %SolutionDir%KL2-Core\KProcess.Ksmed.Presentation.Shell\%OutDir%Extensions\0K-process
xcopy /Y %TargetDir%Resources\exportexcelKp.png  %SolutionDir%KL2-Core\KProcess.Ksmed.Presentation.Shell\%OutDir%Extensions\0K-process\Resources
xcopy /Y %TargetDir%Video\videosplitter-32.exe  %SolutionDir%KL2-Core\KProcess.Ksmed.Presentation.Shell\%OutDir%Extensions\0K-process\Video
xcopy /Y %TargetDir%Video\videosplitter-64.exe  %SolutionDir%KL2-Core\KProcess.Ksmed.Presentation.Shell\%OutDir%Extensions\0K-process\Video
xcopy /Y %TargetDir%Video\arial.ttf  %SolutionDir%KL2-Core\KProcess.Ksmed.Presentation.Shell\%OutDir%Extensions\0K-process\Video
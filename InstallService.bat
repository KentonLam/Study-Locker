@echo off
CD /D %~dp0
:start
echo ======= INSTALLING =======
"C:\windows\Microsoft.NET\Framework64\v4.0.30319\InstallUtil.exe" "StudyLockerApp\StudyLockerService\bin\Debug\StudyLockerService.exe"
echo ======= INSTALLED =======
echo Press enter to uninstall...
pause

echo ======= UNINSTALLING =======
"C:\windows\Microsoft.NET\Framework64\v4.0.30319\InstallUtil.exe" /u "StudyLockerApp\StudyLockerService\bin\Debug\StudyLockerService.exe"
echo ======= UNINSTALLED =======
echo Press enter to re-install...
pause


GOTO start
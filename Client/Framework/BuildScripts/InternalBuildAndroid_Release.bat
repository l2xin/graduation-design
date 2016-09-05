rem copy file
echo off

set unity3dPath=%UNITY_5%
set pythonPath=%PYTHON_PATH%

set projectPath="%~dp0\.."
set MajorVersion=6
set MinorVersion=0
set FixVersion=99
set VersionCode=60099

cd /d %~dp0
%PYTHON_PATH% getsvnrev.py >temp.txt
set /p BuildNo=<temp.txt

echo %BuildNo%


set ScriptingDefineSymbols=RDM_BUILD^|RELEASE_BUILD
echo %PYTHON_PATH%
rem clear bin folder
del /Q "%~dp0..\Bin\*.apk"
%pythonPath% "%~dp0/BuildRDMAndroid.py"
rem copy local bin to parent bin folder
copy /B /Y "%~dp0..\Bin\*.apk" "%~dp0..\..\bin\"

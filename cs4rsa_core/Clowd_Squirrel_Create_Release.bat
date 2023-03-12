echo off
@REM Run this script in the CMD when release
@REM https://github.com/clowd/Clowd.Squirrel
@REM Create Release

echo ======================================================================
echo = Crediz Deploy script                                               =
echo = Create a Squirrel release using the Squirrel.exe command line tool =
echo ======================================================================

set packId=CredizPackId
set packVersion=1.0.0 
set packDirectory=C:\Users\truon\source\repos\cs4rsa_core\cs4rsa_core\bin\Debug\net6.0-windows\publish
set framework=net6.0-x64


echo Run Squirrel.exe
set SquirrelReleaseCommand=C:\Users\truon\.nuget\packages\clowd.squirrel\2.9.42\tools\Squirrel.exe pack --packId=%packId% --packVersion=%packVersion% --packDirectory=%packDirectory% --framework=%framework%
call %SquirrelReleaseCommand%

echo =============================
echo = Create Release Successed! =
echo =============================

echo off
cls
@REM Run this script in the CMD when release
@REM https://github.com/clowd/Clowd.Squirrel
@REM Create Release

call :PrintGreenText ===============================================================================
call :PrintGreenText =                                 Crediz Deploy script                        =
call :PrintGreenText =          Create a Squirrel release using the Squirrel.exe command line tool =
call :PrintGreenText ===============================================================================

@REM Application Name show in Icon, Control Panel
set packId=Crediz
@REM Change packVersion when change version of application to apply update
set packVersion=1.0.2
set packAuthor="CS4RSA by Truong A Xin"
set framework=net6,vcredist143-x86
set pathSquirrel=%userprofile%\.nuget\packages\clowd.squirrel\2.9.42\tools\Squirrel.exe
set pathIconForSetupAndUpdate=AppResources\logo.ico
set pathSplashImage=AppResources\setupSplashScreen.gif
set pathReleaseDir=Releases\
set packDirectory=bin\Release\net6.0-windows\publish

call :PrintYellowText ======================================================================
call :PrintYellowText =                1. Run dotnet build And donet publish               =
call :PrintYellowText ======================================================================
set dotnetBuildAndPublishCommand= dotnet publish ^
        --configuration Release ^
        --no-dependencies ^
        --output %packDirectory%
call dotnet msbuild
call %dotnetBuildAndPublishCommand%

call :PrintYellowText ======================================================================
call :PrintYellowText =                        2. Run Squirrel pack                        =
call :PrintYellowText ======================================================================
set SquirrelReleaseCommand=%pathSquirrel% pack ^
    --packId=%packId% ^
    --packVersion=%packVersion% ^
    --packDirectory=%packDirectory% ^
    --packAuthors=%packAuthor% ^
    --framework=%framework% ^
    --icon=%pathIconForSetupAndUpdate% ^
    --splashImage=%pathSplashImage% ^
    --releaseDir=%pathReleaseDir%
call %SquirrelReleaseCommand%

call :PrintGreenText ================================
call :PrintGreenText =  Create Release Successed!   =
call :PrintGreenText ================================

echo Let push all file in the followed folder to Github release
echo Check out the %pathReleaseDir% folder
call explorer %pathReleaseDir%

@REM Print to console a green text
:PrintGreenText
echo [92m%*[0m
exit /B 0

@REM Print to console a yellow text
:PrintYellowText
echo [93m%*[0m
exit /B 0
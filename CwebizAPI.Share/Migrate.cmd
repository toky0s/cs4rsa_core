@REM Cài hai package Microsoft.EntityFrameworkCore.Design và Microsoft.EntityFrameworkCore.SqlServer

@echo off
cls

@REM Thư mục chứa file SQL Migration, nhớ thêm * ở cuối
set MigrationPath=C:\Users\truon\source\repos\cs4rsa_core\CwebizAPI.Share\Database\Migrations\*
@REM Thư mục chứa file csproj khởi chạy
set StartUpProject=C:\Users\truon\source\repos\cs4rsa_core\CwebizAPI.Share
@REM Thư mục chứa dự án
set ProjectPath=C:\Users\truon\source\repos\cs4rsa_core\CwebizAPI.Share
@REM Thư mục chứa file model được gen ra.
set ModelFolder=C:\Users\truon\source\repos\cs4rsa_core\CwebizAPI.Share\Database\Models
@REM Connection String
set DbConnectionString=Data Source=DESKTOP-GD9867B\SQLEXPRESS;Initial Catalog=Cwebiz;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False
set Provider=Microsoft.EntityFrameworkCore.SqlServer
set MigrationCommand=dotnet ef dbcontext scaffold "%DbConnectionString%" "%Provider%" --output-dir "%ModelFolder%" --project "%StartUpProject%" --force
@REM Tên server instance
set MyServerInstanceName=DESKTOP-GD9867B\SQLEXPRESS
set DbName=Cwebiz 

echo =============================================
echo Run migrations in %MyServerInstanceName%
echo =============================================
for %%i in (%MigrationPath%) do (
    @REM Chạy sql trên các file trong thư mục Migration
    sqlcmd -S %MyServerInstanceName% -i %%i -e -E
)

echo =============================================
echo Build project with "dotnet build"
echo =============================================
dotnet build %StartUpProject%

echo =============================================
echo Remove Model Folder: %ModelFolder%
echo =============================================
RMDIR /S /Q %ModelFolder%

echo =============================================
echo %MigrationCommand%
echo =============================================

@REM Chạy migrate
%MigrationCommand%
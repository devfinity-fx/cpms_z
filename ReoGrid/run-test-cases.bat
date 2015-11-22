@echo off

"C:\Windows\Microsoft.NET\Framework64\v3.5\msbuild" ReoGrid\ReoGrid.csproj /p:Configuration=Debug

"C:\Windows\Microsoft.NET\Framework64\v3.5\msbuild" TestCase\TestCase.csproj /p:Configuration=Release

if not exist ReoGrid\bin\debug\unvell.ReoGrid.dll (
  echo ReoGrid core dll not found!
  echo.
  goto :eof
)

cd TestCase\bin\Release

echo Start run test cases...
echo.

ReoGridUnitTest.exe

if %errorlevel%==1 (
	echo !!!TEST CASE FAILED!!!
	echo.
)

if %errorlevel%==0 (
	echo All test cases passed as expected.
	echo.
)

pause

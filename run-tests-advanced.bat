@echo off
REM DMB Backend Unit Tests Runner - Advanced Version
REM Ez a script futtatja az összes unit tesztet különböző módokkal

setlocal enabledelayedexpansion

color 0A
cls

echo.
echo ======================================================
echo   DMB Backend - Unit Tests Runner
echo   Advanced version with multiple options
echo ======================================================
echo.

REM Menu megjelenítés
:menu
echo [1] Quick run (minimal output)
echo [2] Standard run (normal output)
echo [3] Detailed run (verbose output)
echo [4] Run specific test file
echo [5] Run with coverage report
echo [6] Clean and run
echo [7] Exit
echo.
set /p choice="Select option (1-7): "

if "%choice%"=="1" goto quick_run
if "%choice%"=="2" goto standard_run
if "%choice%"=="3" goto detailed_run
if "%choice%"=="4" goto specific_test
if "%choice%"=="5" goto coverage_run
if "%choice%"=="6" goto clean_run
if "%choice%"=="7" goto end
echo Invalid option. Please try again.
goto menu

:quick_run
cls
echo.
echo [QUICK RUN - Minimal output]
echo.
cd dmb_backend.Tests
dotnet test --logger "console;verbosity=minimal"
cd ..
pause
goto menu

:standard_run
cls
echo.
echo [STANDARD RUN - Normal output]
echo.
cd dmb_backend.Tests
dotnet test --logger "console;verbosity=normal"
cd ..
pause
goto menu

:detailed_run
cls
echo.
echo [DETAILED RUN - Verbose output]
echo.
cd dmb_backend.Tests
dotnet test --logger "console;verbosity=detailed"
cd ..
pause
goto menu

:specific_test
cls
echo.
echo Available test files:
echo [1] AuthControllerTests
echo [2] UserControllerTests
echo [3] NewsControllerAuthorizationTests
echo [4] NewsItemModelTests
echo [5] ModelsTests
echo [6] DTOsTests
echo.
set /p test_choice="Select test file (1-6): "

if "%test_choice%"=="1" set test_name=AuthControllerTests
if "%test_choice%"=="2" set test_name=UserControllerTests
if "%test_choice%"=="3" set test_name=NewsControllerAuthorizationTests
if "%test_choice%"=="4" set test_name=NewsItemModelTests
if "%test_choice%"=="5" set test_name=ModelsTests
if "%test_choice%"=="6" set test_name=DTOsTests

if defined test_name (
    cls
    echo.
    echo Running: !test_name!
    echo.
    cd dmb_backend.Tests
    dotnet test --filter "!test_name!" --logger "console;verbosity=normal"
    cd ..
) else (
    echo Invalid choice!
)
pause
goto menu

:coverage_run
cls
echo.
echo [COVERAGE RUN - With code coverage report]
echo.
echo Note: Requires coverlet installed
echo.
cd dmb_backend.Tests
dotnet test /p:CollectCoverage=true /p:CoverageFormat=opencover
cd ..
pause
goto menu

:clean_run
cls
echo.
echo [CLEAN RUN - Clean, rebuild and test]
echo.
cd dmb_backend.Tests
echo Cleaning...
dotnet clean
echo.
echo Building...
dotnet build
echo.
echo Testing...
dotnet test --logger "console;verbosity=normal"
cd ..
pause
goto menu

:end
cls
echo.
echo Goodbye!
echo.
exit /b 0

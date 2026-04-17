@echo off
REM DMB Backend Unit Tests Runner
REM Ez a script futtatja az összes unit tesztet és megjeleníti az eredményeket

setlocal enabledelayedexpansion

echo.
echo =====================================================
echo  DMB Backend - Unit Tests
echo =====================================================
echo.

REM Ellenőrizzük, hogy a test projekt létezik-e
if not exist "dmb_backend.Tests\dmb_backend.Tests.csproj" (
    echo Hiba: dmb_backend.Tests projekt nem található!
    echo Kérlek futtasd ezt a scriptet a workspace gyökérkönyvtárából.
    exit /b 1
)

echo [INFO] Tesztek futtatása...
echo.

REM Futtatjuk a teszteket
cd dmb_backend.Tests
dotnet test --logger "console;verbosity=minimal"

REM Ellenőrizzük az exit kódot
if %ERRORLEVEL% equ 0 (
    echo.
    echo =====================================================
    echo  [✓] Összes teszt sikeresen átment!
    echo =====================================================
    echo.
) else (
    echo.
    echo =====================================================
    echo  [✗] Hibák történtek a tesztek futtatásakor!
    echo =====================================================
    echo.
    exit /b 1
)

cd ..

REM Futtatjuk a teszteket ismét részletesebben, ha szeretnénk
echo.
set /p detailed="Szeretnéd a részletes output-ot is megtekinteni? (i/n): "
if /i "%detailed%"=="i" (
    echo.
    echo =====================================================
    echo  Részletes teszt eredmények
    echo =====================================================
    echo.
    cd dmb_backend.Tests
    dotnet test --logger "console;verbosity=detailed"
    cd ..
)

echo.
echo Program vége.
pause

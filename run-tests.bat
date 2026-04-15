@echo off
chcp 65001 >nul
cls
echo ========================================
echo   NewsSection komponens TESZT FUTTATÁS
echo ========================================
echo.
cd /d "%~dp0"
echo Tesztek indítása...
echo.
call npm test -- src/components/NewsSection.test.jsx --watchAll=false --coverage
if %ERRORLEVEL% equ 0 (
    echo.
    echo ========================================
    echo ✓ Tesztek sikeresen lezárultak
    echo ========================================
) else (
    echo.
    echo ========================================
    echo ✗ Tesztek hibával zárultak
    echo ========================================
)
pause
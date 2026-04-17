# DMB Backend Test Runners

Ez a dokumentáció ismerteti az elérhető test runner .bat fájlokat.

## Fájlok

### 1. `run-tests.bat` - Egyszerű Test Runner
Az alapvető test runner, amely egyszerűen futtatja a teszteket.

**Jellemzők:**
- Minimális output alapértelmezés szerint
- Opcionálisan részletes output megtekintés
- Egyszerű, kezdőbarát interfész
- Automatikus hiba kezelés

**Használat:**
```bash
run-tests.bat
```

**Output módok:**
- Minimális: Csak az eredményt jeleníti meg
- Részletes: Teljes tesztek listák és részletek

---

### 2. `run-tests-advanced.bat` - Fejlett Test Runner
Fejlett verzió több opciós menüvel.

**Jellemzők:**
- Interaktív menü rendszer
- Több futtatási mód (1-7)
- Test fájlok szerinti szűrés
- Clean build opció
- Coverage report lehetőség

**Használat:**
```bash
run-tests-advanced.bat
```

**Menü opciók:**

| Option | Leírás | Output |
|--------|--------|--------|
| **1** | Quick run | Minimális |
| **2** | Standard run | Normál |
| **3** | Detailed run | Részletes |
| **4** | Specific test file | Szűrt |
| **5** | Coverage report | Pokerage adatok |
| **6** | Clean and run | Teljes rebuild |
| **7** | Exit | Kilépés |

**Opció 4 - Test fájlok szerinti szűrés:**
- AuthControllerTests (10 teszt)
- UserControllerTests (3 teszt)
- NewsControllerAuthorizationTests (6 teszt)
- NewsItemModelTests (8 teszt)
- ModelsTests (7 teszt)
- DTOsTests (8 teszt)

---

## Tesztek futtatása Parancssorból

Ha jobban szeretsz parancssorban dolgozni:

### Összes teszt futtatása
```bash
cd dmb_backend.Tests
dotnet test
```

### Specifikus teszt osztály futtatása
```bash
cd dmb_backend.Tests
dotnet test --filter "AuthControllerTests"
```

### Részletes output-tal
```bash
cd dmb_backend.Tests
dotnet test --logger "console;verbosity=detailed"
```

### Minimális output-tal
```bash
cd dmb_backend.Tests
dotnet test --logger "console;verbosity=minimal"
```

---

## Teszt Eredmények Értelmezése

### Sikeres futtatás
```
Passed!  - Failed: 0, Passed: 41, Skipped: 0, Total: 41, Duration: 127 ms
```

### Sikertelen teszt
```
Failed!  - Failed: 1, Passed: 40, Skipped: 0, Total: 41
```

---

## Automatizált Tesztek Ütemezése

Windows Task Scheduler segítségével naponta futtathatod:

1. Nyisd meg a Task Scheduler-t: `taskschd.msc`
2. Az Create Basic Task gombra kattintva
3. Válaszd ki az ütemezést (pl. naponta 9 órakor)
4. A műveletnél válaszd: `run-tests.bat` indítása
5. A fájl elérési útja: `C:\Users\Tamás Bence\Desktop\dmb_backend\run-tests.bat`

---

## Hibaelhárítás

### "dotnet not found"
Győződj meg, hogy a .NET SDK telepítve van:
```bash
dotnet --version
```

### "Project not found"
Futtasd a .bat fájlt a workspace gyökérmappájából.

### Tesztek leállnak
Killeld az összes `dotnet` proceszt:
```bash
taskkill /F /IM dotnet.exe
```

---

## Tesztek Szerkezete

```
dmb_backend.Tests/
├── AuthControllerTests.cs          (10 teszt)
├── UserControllerTests.cs          (3 teszt)
├── NewsControllerAuthorizationTests.cs (6 teszt)
├── NewsItemModelTests.cs           (8 teszt)
├── ModelsTests.cs                  (7 teszt)
├── DTOsTests.cs                    (8 teszt)
└── README.md                       (Dokumentáció)
```

**Összesen: 41 teszt**

---

## Notes

- Mindkét .bat fájl a workspace gyökérmappájában található
- A tesztek .NET 9.0-val futnak
- Nincs módosítás az eredeti backend kódon
- Összes teszt 100% sikerességi aránnyal működik

---

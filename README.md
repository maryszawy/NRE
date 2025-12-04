# NRE

## Architektura i Ścieżki Zapisu

W przeciwieństwie do standardowych praktyk Unity, ten projekt **nie używa** `Application.persistentDataPath` (czyli AppData). Zamiast tego zapisuje stan gry bezpośrednio w strukturze katalogów gry.

### Save'y
Logika znajduje się w skrypcie `SciezkiZapisu.cs`.
* **Katalog zapisu:** `Application.dataPath + "/Data/Save"`
* **W Edytorze Unity:** Pliki pojawią się w folderze `Assets/Data/Save/`.
* **W zbudowanej grze:** W folderze `NazwaGry_Data/Data/Save/`.

### Pliki danych:
1.  **`Assets/StreamingAssets/miasta.json`** – **Baza danych świata.** Plik tylko do odczytu (dla gry). Zawiera definicje miast, połączeń i startowe towary.
2.  **`Assets/Data/Save/miasta.json`** – **Stan świata (Save).** Tworzony dynamicznie. Uwaga: ma taką samą nazwę jak plik bazowy, ale leży w innym folderze! To tutaj zapisywane są zmiany cen i ilości towarów (sekcja `after`).
3.  **`Assets/Data/Save/player.json`** – **Stan gracza.** Tu ląduje Twoje złoto i ekwipunek.

> **Wskazówka developerska:** Dzięki temu, że save'y są w `Assets/`, możesz je edytować ręcznie w Visual Studio/Notatniku i Unity natychmiast je zobaczy. Aby zresetować grę, po prostu usuń zawartość folderu `Assets/Data/Save/`.

---

## Wymagania Techniczne

* **Unity:** 6000.2.7f2 (zalecana)
* **Paczki:** Universal Render Pipeline (URP), Newtonsoft.Json (wymagany do serializacji danych).

---

## Mechanika Rynku

Ekonomia w `RynekMiast.Multipliers.cs` nie jest losowa. Działa na krzywej popytu i podaży:

### 1. Kupno i Sprzedaż
* **Kupno:** Płacisz 100% ceny bazowej zdefiniowanej w danym mieście.
* **Sprzedaż:** Cena, którą otrzymasz, zależy od **nasycenia rynku** (`stockRatio = ilość / ilość_bazowa`).

### 2. Mnożniki zarobku (Algorytm)
Skrypt analizuje, ile miasto ma towaru w porównaniu do normy (`regular_quantity`):
* **Braki (< 10% zapasów):** Sprzedasz za **80-90%** ceny bazowej (Najlepszy zysk!).
* **Norma (40-80% zapasów):** Sprzedasz za **30-50%** ceny.
* **Przesyt (> 80% zapasów):** Sprzedasz za grosze (**0-25%**).

> **Wniosek:** Szukaj miast, gdzie `quantity` jest bliskie 0.

### 3. Bonus Fabryczny
Jeśli miasto posiada fabrykę produkującą dany typ towaru (np. `mine` dla `metal`), mnożnik sprzedaży jest podbijany o stałe **5%** (0.05).

---

## System Udźwigu

Twoja ładowność jest ściśle limitowana (`maksUdzwig = 1000.0`). Każdy towar ma inną wagę (`StanGracza.cs`):

| Towar | Waga (j.) | Komentarz |
| :--- | :--- | :--- |
| **Gems** | 1.0 | Najlżejsze, idealne do szybkiego wzbogacenia. |
| **Food** | 2.0 | - |
| **Fuel** | 3.0 | - |
| **Metal** | 5.0 | - |
| **Relics** | 10.0 | - |

---

## Szybki Start

1.  Otwórz scenę `Assets/Scenes/SampleScene.unity`.
2.  Upewnij się, że folder `Assets/Data/Save` istnieje (lub pozwól grze go utworzyć).
3.  Uruchom grę.
4.  Wybierz miasto docelowe.
5.  Zatwierdź podróż (zwróć uwagę na czas podróży).
6.  Po dotarciu zapłać cło (Fee).
7.  Otwórz handel (Przycisk w UI).
8.  Obserwuj konsolę – zobaczysz logi o tworzeniu plików w `Data/Save`.

---

## Znane problemy

* **Zapisy w `Assets/`:** Ponieważ gra zapisuje pliki wewnątrz folderu projektu, system kontroli wersji (Git) może wykrywać Twoje save'y jako zmiany w kodzie.
    * *Rozwiązanie:* Dodano `Assets/Data/Save/` do `.gitignore` (sprawdź, czy tam jest!).
* **Brak walidacji JSON:** Jeśli ręcznie zepsujesz strukturę `miasta.json`, gra może wczytać puste dane lub wyrzucić błąd deserializacji.
* **UI Hardcoding:** Panel handlu (`PanelMiastoInfo`) odświeża się tylko przy zdarzeniach zmiany złota/ekwipunku.

---

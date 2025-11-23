# NRE

## Cel gry
Gra skupia si na eksploracji mapy miast, opacie podatku za wejcie do wybranego miasta i handlu dobrami (metal, gems, food, fuel, relics). Poruszasz si midzy miastami po zaplanowanych cigach, uzupeaniasz zapasy oraz zarzdzasz złotem i udwigiem ekwipunku, korzystajc z dynamicznie aktualizowanego rynku.

## Wymagania
- Unity **6000.2.7f2**.【F:ProjectSettings/ProjectVersion.txt†L1-L2】
- Universal Render Pipeline (URP) **17.2.0**.【F:Packages/manifest.json†L13-L17】
- Input System **1.14.2** (asset `Assets/InputSystem_Actions.inputactions`).【F:Packages/manifest.json†L11-L16】

## Szybki start
1. Otwórz projekt w wymaganej wersji Unity i pozwól na doimportowanie paczek.
2. Załaduj scenę `Assets/Scenes/SampleScene.unity` i upewnij się, że obiekty z komponentami `MapaGry`, `RynekMiast` oraz `StanGracza` są obecne w hierarchii.
3. W `MapaGry` wskaż listę miast i obiekt gracza (`GraczController`), a następnie wciśnij **Play**, aby rozpocząć rozgrywkę od miasta startowego lub wylosowanego.
4. Wejście do miasta wymaga opłaty (`fee`) pobieranej z danych rynku, po czym można otworzyć panele handlu i informacji o mieście sterowane przez `MapaGry` i `PanelMiastoInfo`.

## Dane
- `Assets/StreamingAssets/miasta.json` – bazowy opis świata: lista miast z rozmiarami, opłatą za wejście, fabrykami, połączeniami, misjami oraz cenami/zapasami towarów, plus opcjonalna sekcja `after` z zapisanym stanem rynku.【F:Assets/Scripts/RynekMiast.cs†L32-L113】
- `Assets/StreamingAssets/city_rules.json` – parametry generowania/skalowania miast (przedziały opłat, liczby fabryk, zakresy cen i ilości dla poszczególnych rozmiarów).【F:Assets/StreamingAssets/city_rules.json†L1-L37】

### Reset lub podmiana danych rynku
- Bieżący stan rynku jest zapisywany do `miasta_state.json` w `Application.persistentDataPath` w sekcji `after`; plik tworzy `RynekMiast.SaveAfter()` przy każdej zmianie asortymentu.【F:Assets/Scripts/RynekMiast.cs†L47-L113】
- Aby zresetować rynek do danych bazowych, usuń `miasta_state.json` z katalogu danych użytkownika lub wyczyść sekcję `after`, pozostawiając tylko `cities`.
- Aby zasymulować inny stan startowy, edytuj sekcję `after` w `miasta_state.json` (np. wklejając zmodyfikowane wpisy miast); przy następnym uruchomieniu zostanie ona załadowana jako aktualny stan.

### Lokalizacja zapisów
- Rynek: `Application.persistentDataPath/miasta_state.json` (tworzony i nadpisywany przez `RynekMiast`).【F:Assets/Scripts/RynekMiast.cs†L47-L113】
- Gracz: `Application.persistentDataPath/player_state.json` (tworzony przez `StanGracza` przy starcie, aktualizowany przy zmianach złota/ekwipunku).【F:Assets/Scripts/StanGracza.cs†L40-L113】

## Główne sceny i komponenty startowe
- **Scena**: `SampleScene.unity` – główna mapa gry z obiektami sterującymi logiką podróży i handlu.
- **MapaGry** – centralny kontroler mapy; konfiguruje miasto startowe, reaguje na kliknięcia miast, odblokowuje handel po opłacie wejściowej i prezentuje UI powiązane z miastem.【F:Assets/Scripts/MapaGry.cs†L8-L158】
- **RynekMiast** – singleton ładujący/zapisujący dane rynku z `StreamingAssets` lub pliku persist, udostępnia bieżące stany miast dla UI i logiki handlu.【F:Assets/Scripts/RynekMiast.cs†L35-L110】
- **StanGracza** – singleton przechowujący złoto, udźwig i ekwipunek; zapisuje stan do pliku persist.【F:Assets/Scripts/StanGracza.cs†L17-L87】
- **GraczController** – odpowiada za ruch między miastami po wyznaczonej ścieżce i powiadamia `MapaGry` o dotarciu do celu.【F:Assets/Scripts/GraczController.cs†L7-L118】
- **InputSystem_Actions** – asset Input System używany do mapowania sterowania.

## Kluczowe skrypty rozgrywki
- **Ruch i mapa**: `Miasto` odpowiada za hover/kliknięcia i podświetlenia węzłów, odsyłając zdarzenia do `MapaGry`; `Droga` trzyma połączenia między miastami, spina je z LineRendererem i dostarcza punkty trasy oraz długości dla pathfindera.【F:Assets/Scripts/Miasto.cs†L1-L64】【F:Assets/Scripts/Droga.cs†L1-L86】
- **Podróż gracza**: `GraczController` buduje pełne listy punktów na podstawie dróg, emituje eventy postępu i końca podróży oraz ustawia gracza na miasto startowe; przy każdym finiszu wzywa `MapaGry.PoDotarciuDoCelu`.【F:Assets/Scripts/GraczController.cs†L7-L120】
- **Czas i HUD**: `CzasGry` przelicza upływ minut gry z real-time i emituje zdarzenia zmiany; `UIHUD` słucha zegara oraz eventów podróży gracza, pokazuje aktualny dzień/godzinę i postęp trasy (wraz z ETA).【F:Assets/Scripts/CzasGry.cs†L1-L43】【F:Assets/Scripts/UIHUD.cs†L1-L84】
- **Handel**: `PanelMiastoInfo` wypełnia listę towarów, pozwala kupować/sprzedawać (sprawdza złoto, ciężar, dostępność), aktualizuje UI po transakcjach i zapisuje zmiany rynku przez `RynekMiast.AdjustCommodity`; `UIHandelToggle` pilnuje, by panel handlu był dostępny dopiero po wejściu do miasta, reaguje na kliknięcie przycisku lub klawisz `H`.【F:Assets/Scripts/PanelMiastoInfo.cs†L1-L121】【F:Assets/Scripts/UIHandelToggle.cs†L1-L88】
- **Ekonomia i dane**: `RynekMiast` ładuje świat i zapisuje sekcję `after`, a `RynekMiast.Multipliers` oblicza mnożniki cen sprzedaży w zależności od zapasów i fabryk; `StanGracza` śledzi złoto, ekwipunek i udźwig, zapisując je do `player_state.json` i emitując eventy dla UI. Towary i ceny wynikają z danych miast, a dodatkowe aktualizacje trafiają do persist. 【F:Assets/Scripts/RynekMiast.cs†L35-L110】【F:Assets/Scripts/RynekMiast.Multipliers.cs†L1-L44】【F:Assets/Scripts/StanGracza.cs†L1-L87】
- **Wejście do miasta i potwierdzenia**: `PanelWejsciaMiasta` pokazuje opłatę wejściową i wpuszcza gracza po pobraniu złota; `PanelPotwierdzenia` wstrzymuje czas na czas wyboru trasy, wspiera klawiaturę/efekty audio i pozwala anulować podróż jeszcze przed startem.【F:Assets/Scripts/PanelWejsciaMiasta.cs†L1-L59】【F:Assets/Scripts/PanelPotwierdzenia.cs†L1-L88】
- **Widoczność stanu gracza**: `PanelEkwipunku` umożliwia szybkie sprawdzenie ekwipunku oraz obciążenia.【F:Assets/Scripts/PanelEkwipunku.cs†L1-L69】

## Known issues / roadmap
- Brak dedykowanej sceny startowej poza `SampleScene` – rozważyć wydzielenie menu lub sceny testowej.
- Przy pierwszym uruchomieniu z pustym `PersistentDataPath` pliki `player_state.json` i `miasta_state.json` są tworzone automatycznie; dodanie przycisku resetu w UI ułatwiłoby czyszczenie zapisów.
- Logika gry jest silnie sprzężona z UI i komponentami sceny, co ogranicza możliwość testów jednostkowych lub pracy bez edytora Unity

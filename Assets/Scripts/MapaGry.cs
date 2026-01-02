using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MapaGry : MonoBehaviour
{
    public static MapaGry Instance { get; private set; }

    [Header("Scena")]
    public List<Miasto> listaMiast = new List<Miasto>();
    public GraczController gracz;
    public LineRenderer podgladSciezki;
    public SpriteRenderer spriteMapy;

    private Miasto _ostatnioKlikniete;
    private KameraMapa _kamera;
    private System.Random _rng = new System.Random();

    [Header("Start gry")]
    public bool losowyStart = false;
    public Miasto startoweMiasto;

    [Header("UI")]
    public PanelPotwierdzenia panelPotwierdzenia;
    public PanelMiastoInfo panelMiastoInfo;
    public PanelWejsciaMiasta panelWejsciaMiasta;
    public PanelZdarzenia panelZdarzenia;

    [Header("UI — przyciski")]
    public Button btnHandel;

    [Header("Gameplay")]
    public bool moznaHandlowac = false;

    public bool CzyMapaZablokowana =>
        (panelMiastoInfo != null && panelMiastoInfo.Widoczny)
     || (panelPotwierdzenia != null && panelPotwierdzenia.Widoczny)
     || (panelWejsciaMiasta != null && panelWejsciaMiasta.Widoczny)
     || (panelZdarzenia != null && panelZdarzenia.Widoczny);

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        _kamera = Camera.main?.GetComponent<KameraMapa>();
    }

    private void Start()
    {
        if (listaMiast.Count == 0)
        {
            Debug.LogError("Brak miast na mapie!");
            return;
        }

        Miasto startMiasto = (!losowyStart && startoweMiasto != null)
            ? startoweMiasto
            : listaMiast[_rng.Next(listaMiast.Count)];

        gracz.UstawStart(startMiasto);

        if (_kamera && spriteMapy != null)
            _kamera.SkonfigurujGranice(spriteMapy.bounds);

        PodkreslMiasto(startMiasto);

        moznaHandlowac = false;
        if (btnHandel) btnHandel.interactable = false;
    }

    //=============== KLIK NA MIASTO ============================
    public void KliknietoMiasto(Miasto miasto)
    {
        if (gracz == null || gracz.czyWTrasie) return;
        if (CzyMapaZablokowana) return;

        _ostatnioKlikniete = miasto;
        WyczyscPodswietlenia();
        miasto.UstawZaznaczenie(true);

        var sciezka = ZnajdzSciezke(gracz.AktualneMiasto, miasto);
        if (sciezka == null)
        {
            PodgladujSciezke(null);
            Debug.LogWarning($"Brak drogi do {miasto.nazwa}");
            return;
        }

        PodgladujSciezke(sciezka);

        var (_, _, minGry) = ObliczCzasPodrozy(sciezka);
        string eta = FormatujMinutyGry(minGry);

        panelPotwierdzenia.Pokaz(
            gracz.AktualneMiasto.nazwa,
            miasto.nazwa,
            eta,
            poPotwierdzeniu: () => gracz.RuszajPoSciezce(sciezka),
            poAnulowaniu: () => { PodgladujSciezke(null); WyczyscPodswietlenia(); }
        );
    }

    //=============== PO DOJECHANIU — EVENT + MIASTO =============
    public void PoDotarciuDoCelu(Miasto miasto)
    {
        PodgladujSciezke(null);

        if (ZdarzeniaManager.Instance != null && ZdarzeniaManager.Instance.CzyJestZdarzenieDoOdpalenia())
        {
            ZdarzeniaManager.Instance.ObsluzZdarzeniePrzyPrzyjezdzie(
                miasto,
                onZakonczenie: () => KontynuujWejscieDoMiasta(miasto)
            );
        }
        else
        {
            KontynuujWejscieDoMiasta(miasto);
        }
    }

    //=============== WEJŒCIE DO MIASTA PO EVENCIE ================
    private void KontynuujWejscieDoMiasta(Miasto miasto)
    {
        var dataMiasta = RynekMiast.Instance?.FindCity(miasto.nazwa);

        if (panelWejsciaMiasta == null || dataMiasta == null)
        {
            moznaHandlowac = true;
            btnHandel.interactable = true;
            panelMiastoInfo.Pokaz(miasto.nazwa);
            return;
        }

        int fee = dataMiasta.fee;
        panelWejsciaMiasta.Pokaz(
            miasto.nazwa,
            fee,
            poWejsciu: () =>
            {
                if (StanGracza.Instance.dane.zloto < fee) return;

                StanGracza.Instance.DodajZloto(-fee);
                moznaHandlowac = true;
                btnHandel.interactable = true;
                panelMiastoInfo.Pokaz(miasto.nazwa);
            },
            poOdejscie: () =>
            {
                moznaHandlowac = false;
                btnHandel.interactable = false;
                panelMiastoInfo.Ukryj();
            }
        );
    }


    private void PodkreslMiasto(Miasto m) => m?.UstawZaznaczenie(true);

    private void WyczyscPodswietlenia()
    {
        if (listaMiast == null || listaMiast.Count == 0)
        {
            Debug.LogWarning("MapaGry: listaMiast jest pusta – upewnij siê, ¿e podpi¹³eœ miasta w Inspectorze.");
            return;
        }

        foreach (var m in listaMiast)
        {
            if (m != null) m.UstawZaznaczenie(false);
        }
    }

    private float Dystans(Miasto a, Miasto b) => Vector2.Distance(a.transform.position, b.transform.position);

    public List<Miasto> ZnajdzSciezke(Miasto start, Miasto cel)
    {
        if (start == null || cel == null) return null;

        var odwiedzone = new HashSet<Miasto>();
        var dystans = new Dictionary<Miasto, float>();
        var poprzednik = new Dictionary<Miasto, Miasto>();

        foreach (var m in listaMiast) dystans[m] = float.PositiveInfinity;
        dystans[start] = 0f;

        while (true)
        {
            Miasto u = null;
            var min = float.PositiveInfinity;

            foreach (var m in listaMiast)
            {
                if (!odwiedzone.Contains(m) && dystans[m] < min)
                {
                    min = dystans[m];
                    u = m;
                }
            }

            if (u == null) break;
            if (u == cel) break;
            odwiedzone.Add(u);

            foreach (var d in u.drogi)
            {
                if (d == null) continue;

                Miasto v = (d.miastoA == u) ? d.miastoB : d.miastoA;
                if (v == null) continue;

                var alt = dystans[u] + d.Dlugosc();
                if (alt < dystans[v])
                {
                    dystans[v] = alt;
                    poprzednik[v] = u;
                }
            }
        }

        if (!poprzednik.ContainsKey(cel) && start != cel) return null;

        var sciezka = new List<Miasto>();
        var x = cel;
        sciezka.Add(x);
        while (x != start)
        {
            if (!poprzednik.TryGetValue(x, out var p)) break;
            x = p;
            sciezka.Add(x);
        }
        sciezka.Reverse();
        return sciezka;
    }

    private void PodgladujSciezke(List<Miasto> sciezka)
    {
        if (podgladSciezki == null) return;

        if (sciezka == null || sciezka.Count < 2)
        {
            podgladSciezki.positionCount = 0;
            return;
        }

        podgladSciezki.positionCount = sciezka.Count;
        for (int i = 0; i < sciezka.Count; i++)
            podgladSciezki.SetPosition(i, sciezka[i].transform.position);
    }

    private Droga ZnajdzDroge(Miasto a, Miasto b)
    {
        foreach (var d in a.drogi)
            if ((d.miastoA == a && d.miastoB == b) || (d.miastoA == b && d.miastoB == a))
                return d;
        return null;
    }

    private (float dystans, float sekReal, float minGry) ObliczCzasPodrozy(List<Miasto> sciezka)
    {
        if (sciezka == null || sciezka.Count < 2 || gracz == null) return (0, 0, 0);

        float dystans = 0f;
        for (int i = 0; i < sciezka.Count - 1; i++)
        {
            var d = ZnajdzDroge(sciezka[i], sciezka[i + 1]);
            if (d != null) dystans += d.Dlugosc();
        }

        float sekReal = dystans / Mathf.Max(0.0001f, gracz.predkosc);
        float minGry = sekReal * ((CzasGry.Instance != null) ? CzasGry.Instance.minutyNaSekunde : 1f);
        return (dystans, sekReal, minGry);
    }

    private string FormatujMinutyGry(float min)
    {
        int m = Mathf.RoundToInt(min);
        int h = m / 60;
        int mm = m % 60;
        return (h > 0) ? $"{h}h {mm}m" : $"{mm}m";
    }
}

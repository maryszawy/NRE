using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PanelMiastoInfo : MonoBehaviour
{
    [Header("UI")]
    public GameObject kontener;
    public TextMeshProUGUI txtTytul;
    public Transform content;
    public GameObject prefabWiersz;
    public Button btnZamknij;

    public bool Widoczny => kontener != null && kontener.activeSelf;

    private string _miasto;
    private Dictionary<string, WierszTowaruUI> _wiersze = new();

    private void Awake()
    {
        if (btnZamknij) btnZamknij.onClick.AddListener(Ukryj);
        Ukryj();
    }

    private void OnEnable()
    {
        if (StanGracza.Instance != null)
        {
            StanGracza.Instance.OnZlotoZmiana += OnZloto;
            StanGracza.Instance.OnEkwipunekZmiana += OnEkwipunek;
        }
    }

    private void OnDisable()
    {
        if (StanGracza.Instance != null)
        {
            StanGracza.Instance.OnZlotoZmiana -= OnZloto;
            StanGracza.Instance.OnEkwipunekZmiana -= OnEkwipunek;
        }
    }

    void OnZloto(float _) { if (Widoczny) OdswiezWidoczne(); }
    void OnEkwipunek(string nazwa, int _) { if (Widoczny) OdswiezWiersz(nazwa); }

    public void Pokaz(string nazwaMiasta)
    {
        _miasto = nazwaMiasta;

        var miasto = RynekMiast.Instance?.FindCity(nazwaMiasta);
        if (miasto == null) { Debug.LogWarning("[PanelMiastoInfo] Brak miasta: " + nazwaMiasta); return; }

        if (txtTytul) txtTytul.text = $"{miasto.name}";

        foreach (Transform t in content) Destroy(t.gameObject);
        _wiersze.Clear();

        if (miasto.commodities != null)
        {
            var pary = miasto.commodities.Where(kv => kv.Key != "special");
            foreach (var kv in pary)
            {
                var go = Instantiate(prefabWiersz, content);
                var ui = go.GetComponent<WierszTowaruUI>();

                ui.nazwaTowaru = kv.Key;

                float cenaKupna = RynekMiast.ObliczCeneKupna(miasto, kv.Key);
                float wyplata = RynekMiast.ObliczWyplateSprzedazy(miasto, kv.Key);

                ui.txtNazwa.text = kv.Key;

                ui.txtCena.text = $"Cena: {cenaKupna:F2} | Skup: ~{wyplata:F2}";

                ui.txtMiastoIlosc.text = $"Miasto: {kv.Value.quantity}";
                ui.txtGraczIlosc.text = $"Ty: {StanGracza.Instance.IleTowaru(kv.Key)}";

                ui.btnKup.onClick.RemoveAllListeners();
                ui.btnKup.onClick.AddListener(() => KupJednostke(kv.Key));

                ui.btnSprzedaj.onClick.RemoveAllListeners();
                ui.btnSprzedaj.onClick.AddListener(() => SprzedajJednostke(kv.Key));

                _wiersze[kv.Key] = ui;
            }
        }

        kontener.SetActive(true);
    }

    public void Ukryj()
    {
        kontener.SetActive(false);
    }

    void KupJednostke(string towar)
    {
        var miasto = RynekMiast.Instance?.FindCity(_miasto);
        if (miasto == null || miasto.commodities == null || !miasto.commodities.ContainsKey(towar)) return;

        var c = miasto.commodities[towar];
        float cena = RynekMiast.ObliczCeneKupna(miasto, towar);

        if (c.quantity <= 0)
        {
            Debug.Log("Miasto nie ma juz tego towaru.");
            return;
        }

        if (StanGracza.Instance.dane.zloto < cena)
        {
            Debug.Log("Za malo zlota.");
            return;
        }

        float wagaJednostki = StanGracza.Instance.PobierzWageTowaru(towar);
        float aktualneObc = StanGracza.Instance.AktualneObciazenie;
        float maxObc = StanGracza.Instance.maksUdzwig;

        if (aktualneObc + wagaJednostki > maxObc)
        {
            Debug.Log("Nie uniesiesz wiecej tego towaru – przekroczysz maksymalny udzwig.");
            return;
        }

        StanGracza.Instance.DodajZloto(-cena);
        StanGracza.Instance.DodajTowar(towar, +1);
        RynekMiast.Instance.AdjustCommodity(_miasto, towar, -1);

        OdswiezWiersz(towar);
    }


    void SprzedajJednostke(string towar)
    {
        var miasto = RynekMiast.Instance?.FindCity(_miasto);
        if (miasto == null || miasto.commodities == null || !miasto.commodities.ContainsKey(towar)) return;

        if (StanGracza.Instance.IleTowaru(towar) <= 0) { Debug.Log("Nie masz tego towaru."); return; }

        float wyplata = RynekMiast.ObliczWyplateSprzedazy(miasto, towar);

        StanGracza.Instance.DodajZloto(+wyplata);
        StanGracza.Instance.DodajTowar(towar, -1);
        RynekMiast.Instance.AdjustCommodity(_miasto, towar, +1);

        OdswiezWiersz(towar);
    }

    void OdswiezWiersz(string towar)
    {
        if (!_wiersze.TryGetValue(towar, out var ui)) return;

        var m = RynekMiast.Instance.FindCity(_miasto);
        if (m == null || m.commodities == null || !m.commodities.ContainsKey(towar)) return;

        var c = m.commodities[towar];
        float cenaKupna = RynekMiast.ObliczCeneKupna(m, towar);
        float wyplata = RynekMiast.ObliczWyplateSprzedazy(m, towar);

        ui.txtCena.text = $"Cena: {cenaKupna:F2}  |  Skup: ~{wyplata:F2}";

        ui.txtMiastoIlosc.text = $"Miasto: {c.quantity}";
        ui.txtGraczIlosc.text = $"Ty: {StanGracza.Instance.IleTowaru(towar)}";
    }

    void OdswiezWidoczne()
    {
        foreach (var k in _wiersze.Keys.ToList())
            OdswiezWiersz(k);
    }
}

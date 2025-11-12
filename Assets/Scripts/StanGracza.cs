using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

[Serializable]
public class DaneGracza
{
    public int zloto = 500;

    public Dictionary<string, int> ekwipunek = new Dictionary<string, int> {
        {"metal",0}, {"gems",0}, {"food",0}, {"fuel",0}, {"relics",0}
    };
}

public class StanGracza : MonoBehaviour
{
    public static StanGracza Instance { get; private set; }

    public DaneGracza dane = new DaneGracza();

    public event Action<int> OnZlotoZmiana;
    public event Action<string, int> OnEkwipunekZmiana;

    private string sciezkaPliku => Path.Combine(Application.persistentDataPath, "player_state.json");

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        Wczytaj();

        OnZlotoZmiana?.Invoke(dane.zloto);
        if (dane.ekwipunek != null)
            foreach (var kv in dane.ekwipunek)
                OnEkwipunekZmiana?.Invoke(kv.Key, kv.Value);
    }

    public void DodajZloto(int ilosc)
    {
        dane.zloto = Math.Max(0, dane.zloto + ilosc);
        OnZlotoZmiana?.Invoke(dane.zloto);
        Zapisz();
    }

    public int IleTowaru(string nazwa)
    {
        if (dane.ekwipunek == null) return 0;
        return dane.ekwipunek.TryGetValue(nazwa, out var v) ? v : 0;
    }

    public void DodajTowar(string nazwa, int delta)
    {
        if (dane.ekwipunek == null) dane.ekwipunek = new Dictionary<string, int>();
        if (!dane.ekwipunek.ContainsKey(nazwa)) dane.ekwipunek[nazwa] = 0;

        dane.ekwipunek[nazwa] = Math.Max(0, dane.ekwipunek[nazwa] + delta);
        OnEkwipunekZmiana?.Invoke(nazwa, dane.ekwipunek[nazwa]);
        Zapisz();
    }

    public void Zapisz()
    {
        var json = JsonConvert.SerializeObject(dane, Formatting.Indented);
        File.WriteAllText(sciezkaPliku, json);
    }

    public void Wczytaj()
    {
        if (!File.Exists(sciezkaPliku))
        {
            dane = new DaneGracza();
            Zapisz();
            return;
        }

        try
        {
            var json = File.ReadAllText(sciezkaPliku);
            dane = JsonConvert.DeserializeObject<DaneGracza>(json);
        }
        catch
        {
            dane = new DaneGracza();
        }

        if (dane == null) dane = new DaneGracza();
        if (dane.ekwipunek == null) dane.ekwipunek = new Dictionary<string, int>();

        foreach (var k in new[] { "metal", "gems", "food", "fuel", "relics" })
            if (!dane.ekwipunek.ContainsKey(k)) dane.ekwipunek[k] = 0;
    }
}

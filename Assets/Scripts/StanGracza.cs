using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

[Serializable]
public class DaneGracza
{
    public int zloto = 15000;

    public Dictionary<string, int> ekwipunek = new Dictionary<string, int> {
        {"metal",100}, {"gems",20}, {"food",50}, {"fuel",50}, {"relics",0}
    };
}

public class StanGracza : MonoBehaviour
{
    public static StanGracza Instance { get; private set; }

    public DaneGracza dane = new DaneGracza();

    [Header("Udüwig")]
    [Tooltip("Maksymalny udüwig ekwipunku")]
    public float maksUdzwig = 10f;

    Dictionary<string, float> wagiTowarow = new Dictionary<string, float>
    {
        {"metal", 5f},
        {"gems",  1f},
        {"food",  2f},
        {"fuel",  3f},
        {"relics",10f}
    };

    public event Action<float, float> OnObciazenieZmiana;
    public event Action<int> OnZlotoZmiana;
    public event Action<string, int> OnEkwipunekZmiana;

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

        OnObciazenieZmiana?.Invoke(AktualneObciazenie, maksUdzwig);
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

        OnObciazenieZmiana?.Invoke(AktualneObciazenie, maksUdzwig);
    }

    public void Zapisz()
    {
        if (!Directory.Exists(SciezkiZapisu.Folder))
            Directory.CreateDirectory(SciezkiZapisu.Folder);

        var json = JsonConvert.SerializeObject(dane, Formatting.Indented);
        File.WriteAllText(SciezkiZapisu.PlikGracza, json);

        Debug.Log("Zapisano stan gracza " + SciezkiZapisu.PlikGracza);
    }


    public void Wczytaj()
    {
        if (File.Exists(SciezkiZapisu.PlikGracza))
        {
            var json = File.ReadAllText(SciezkiZapisu.PlikGracza);
            dane = JsonConvert.DeserializeObject<DaneGracza>(json);
        }
        else
        {
            Debug.Log("TworzÍ nowy plik save ó brak istniejπcego");
            dane = new DaneGracza();
            Zapisz();
        }

        if (dane == null) dane = new DaneGracza();
        if (dane.ekwipunek == null) dane.ekwipunek = new Dictionary<string, int>();

        foreach (var k in new[] { "metal", "gems", "food", "fuel", "relics" })
            if (!dane.ekwipunek.ContainsKey(k))
                dane.ekwipunek[k] = 0;
    }


    public float ObliczObciazenie()
    {
        if (dane.ekwipunek == null) return 0f;

        float suma = 0f;
        foreach (var kv in dane.ekwipunek)
        {
            if (wagiTowarow.TryGetValue(kv.Key, out float waga))
                suma += kv.Value * waga;
        }
        return suma;
    }

    public float AktualneObciazenie => ObliczObciazenie();

    public float PobierzWageTowaru(string nazwa)
    {
        return wagiTowarow.TryGetValue(nazwa, out float w) ? w : 0f;
    }
}

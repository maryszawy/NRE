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

    [Header("Udzwig")]
    [Tooltip("Maksymalny udzwig ekwipunku")]
    public float maksUdzwig = 10f;

    Dictionary<string, float> wagiTowarow = new Dictionary<string, float>
    {
        { GameIDs.Metal,  5f },
        { GameIDs.Gems,   1f },
        { GameIDs.Food,   2f },
        { GameIDs.Fuel,   3f },
        { GameIDs.Relics, 10f }
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

    private void OnApplicationQuit()
    {
        Zapisz();
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            Zapisz();
        }
    }

    public void DodajZloto(int ilosc)
    {
        dane.zloto = Math.Max(0, dane.zloto + ilosc);
        OnZlotoZmiana?.Invoke(dane.zloto);
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

        OnObciazenieZmiana?.Invoke(AktualneObciazenie, maksUdzwig);
    }

    public void Zapisz()
    {
        if (!Directory.Exists(SciezkiZapisu.Folder))
            Directory.CreateDirectory(SciezkiZapisu.Folder);

        var json = JsonConvert.SerializeObject(dane, Formatting.Indented);

        try
        {
            File.WriteAllText(SciezkiZapisu.PlikGracza, json);
            Debug.Log($"[StanGracza] Zapisano stan (Zloto: {dane.zloto})");
        }
        catch (Exception ex)
        {
            Debug.LogError("[StanGracza] Blad zapisu!: " + ex.Message);
        }
    }


    public void Wczytaj()
    {
        if (File.Exists(SciezkiZapisu.PlikGracza))
        {
            try
            {
                var json = File.ReadAllText(SciezkiZapisu.PlikGracza);
                dane = JsonConvert.DeserializeObject<DaneGracza>(json);
            }
            catch (Exception ex)
            {
                Debug.LogError("[StanGracza] Blad wczytywania (plik uszkodzony?): " + ex.Message);
                dane = new DaneGracza();
            }
        }
        else
        {
            Debug.Log("Tworze nowy plik save, brak istniejacego");
            dane = new DaneGracza();
            Zapisz();
        }

        if (dane == null) dane = new DaneGracza();
        if (dane.ekwipunek == null) dane.ekwipunek = new Dictionary<string, int>();

        foreach (var k in GameIDs.WszystkieTowary)
        {
            if (!dane.ekwipunek.ContainsKey(k))
                dane.ekwipunek[k] = 0;
        }
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
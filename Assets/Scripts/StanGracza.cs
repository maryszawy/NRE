using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class DaneGracza
{
    public int zloto = 500;
}

public class StanGracza : MonoBehaviour
{
    public static StanGracza Instance { get; private set; }

    public DaneGracza dane = new DaneGracza();
    public event Action<int> OnZlotoZmiana;

    private string sciezkaPliku => Path.Combine(Application.persistentDataPath, "player_state.json");

    [Serializable]
    public class DaneGracza
    {
        public int zloto = 500;
        public Dictionary<string, int> ekwipunek = new Dictionary<string, int>
        {
            {"metal",0}, {"gems",0}, {"food",0}, {"fuel",0}, {"relics",0}
        };
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Wczytaj();
        OnZlotoZmiana?.Invoke(dane.zloto);
    }

    public void DodajZloto(int ilosc)
    {
        dane.zloto += ilosc;
        if (dane.zloto < 0) dane.zloto = 0;
        OnZlotoZmiana?.Invoke(dane.zloto);
        Zapisz();
    }

    public void Zapisz()
    {
        string json = JsonUtility.ToJson(dane, true);
        File.WriteAllText(sciezkaPliku, json);
    }

    public void Wczytaj()
    {
        if (File.Exists(sciezkaPliku))
        {
            dane = JsonUtility.FromJson<DaneGracza>(File.ReadAllText(sciezkaPliku));
        }
        else
        {
            dane = new DaneGracza();
            Zapisz();
        }

        // klucze
        string[] keys = { "metal", "gems", "food", "fuel", "relics" };
        if (dane.ekwipunek == null) dane.ekwipunek = new Dictionary<string, int>();
        foreach (var k in keys) if (!dane.ekwipunek.ContainsKey(k)) dane.ekwipunek[k] = 0;

        OnZlotoZmiana?.Invoke(dane.zloto);
    }

}

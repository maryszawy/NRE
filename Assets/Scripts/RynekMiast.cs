using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class CommodityEntry
{
    [JsonProperty("quantity")] public float quantity;
    [JsonProperty("price")] public float price;
    [JsonProperty("regular_price")] public float regular_price;
    [JsonProperty("regular_quantity")] public float regular_quantity;
    [JsonProperty("special")] public JToken special;
}

[System.Serializable]
public class CityData
{
    [JsonProperty("name")] public string name;
    [JsonProperty("size")] public string size;
    [JsonProperty("factory")] public List<string> factory;
    [JsonProperty("fee")] public int fee;
    [JsonProperty("nr_of_conn")] public string nr_of_conn;
    [JsonProperty("commodities")] public Dictionary<string, CommodityEntry> commodities;
    [JsonProperty("missions")] public int missions;
    [JsonProperty("missions_titles")] public List<string> missions_titles;
    [JsonProperty("connections")] public List<string> connections;
}

[System.Serializable]
public class WorldFile
{
    [JsonProperty("cities")] public List<CityData> cities;
    [JsonProperty("after")] public List<CityData> after;
}

public partial class RynekMiast : MonoBehaviour
{
    public static RynekMiast Instance { get; private set; }

    public WorldFile Data;
    public List<CityData> Current;
    public List<CityData> After;

    private string sciezkaBazowa =>
        Path.Combine(Application.streamingAssetsPath, "miasta.json");

    private string sciezkaStanu =>
        SciezkiZapisu.PlikMiast;

    void Awake()
    {
        Debug.Log("[RynekMiast] Startuj�!");
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadWorld();
    }

    void LoadWorld()
    {
        Debug.Log("Szukam pliku JSON...");
        Debug.Log("�cie�ka StreamingAssets: " + Application.streamingAssetsPath);

        string path = File.Exists(sciezkaStanu) ? sciezkaStanu : sciezkaBazowa;

        Debug.Log("U�ywam �cie�ki: " + path);
        Debug.Log("File.Exists? " + File.Exists(path));
        if (File.Exists(sciezkaBazowa))
            Debug.Log("Zawarto�� StreamingAssets:\n" + string.Join("\n", Directory.GetFiles(Application.streamingAssetsPath)));

        if (!File.Exists(path))
        {
            Debug.LogError("NIE ZNALAZ�EM PLIKU: " + path);
            Data = new WorldFile { cities = new List<CityData>(), after = new List<CityData>() };
            Current = Data.cities;
            After = Data.after;
            return;
        }

        try
        {
            var text = File.ReadAllText(path);
            Debug.Log($"Wczytano tekst JSON ({text.Length} znak�w)");

            var root = JsonConvert.DeserializeObject<WorldFile>(text);
            if (root == null)
            {
                Debug.LogError("Deserializacja zwr�ci�a null!");
                Data = new WorldFile { cities = new List<CityData>(), after = new List<CityData>() };
                Current = Data.cities;
                After = Data.after;
                return;
            }

            Data = root;
            Current = root.cities ?? new List<CityData>();
            After = root.after ?? null;

            if (path == sciezkaBazowa && After != null && After.Count > 0)
            {
                Current = DeepClone(After);
                Debug.Log("U�ywam danych z 'after' jako bie��cego stanu �wiata (z bazy).");

                SaveAfter();
            }

            Debug.Log($"Wczytano Current: {Current.Count} miast, After: {(After != null ? After.Count : 0)}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError("B��d parsowania JSON: " + ex.Message);
            Data = new WorldFile { cities = new List<CityData>(), after = new List<CityData>() };
            Current = Data.cities;
            After = Data.after;
        }
    }

    public void SaveAfter()
    {
        if (Data == null)
            Data = new WorldFile();

        Data.after = DeepClone(Current);

        string folder = Path.GetDirectoryName(sciezkaStanu);
        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        string json = JsonConvert.SerializeObject(Data, Formatting.Indented);
        File.WriteAllText(sciezkaStanu, json);

        Debug.Log("[RynekMiast] Zapisano stan miast do: " + sciezkaStanu);
    }

    public CityData FindCity(string name) => Current.Find(c => c.name == name);

    static T DeepClone<T>(T obj)
    {
        var json = JsonConvert.SerializeObject(obj);
        return JsonConvert.DeserializeObject<T>(json);
    }

    public bool AdjustCommodity(string city, string commodity, int deltaQty, int? newPrice = null)
    {
        var c = FindCity(city);

        if (c == null)
        {
            Debug.LogError($"[RynekMiast] BLAD KRYTYCZNY: Proba modyfikacji towaru w nieistniejacym miescie: '{city}'");
            return false;
        }

        if (c.commodities == null)
        {
            Debug.LogError($"[RynekMiast] BLAD DANYCH: Miasto '{city}' ma pusta sekcje 'commodities' w JSON!");
            return false;
        }

        if (!c.commodities.ContainsKey(commodity))
        {
            Debug.LogError($"[RynekMiast] BLAD DANYCH: Miasto '{city}' nie posiada zdefiniowanego towaru '{commodity}'. Sprawdz plik JSON pod katem literowek!");
            return false;
        }

        var ce = c.commodities[commodity];
        ce.quantity = Mathf.Max(0, ce.quantity + deltaQty);

        if (newPrice.HasValue)
            ce.price = newPrice.Value;

        c.commodities[commodity] = ce;

        SaveAfter();
        return true;
    }
}

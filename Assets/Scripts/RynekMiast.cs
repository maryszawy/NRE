using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

[System.Serializable]
public class CommodityEntry
{
    [JsonProperty("quantity")] public int quantity;
    [JsonProperty("price")] public int price;
    [JsonProperty("regular_price")] public int regular_price;
    [JsonProperty("regular_quantity")] public int regular_quantity;
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

    string StreamingPath => Path.Combine(Application.streamingAssetsPath, "miasta.json");
    string PersistPath => Path.Combine(Application.persistentDataPath, "miasta_state.json");

    void Awake()
    {
        Debug.Log("[RynekMiast] Startujê!");
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadWorld();
    }

    void LoadWorld()
    {
        Debug.Log("Szukam pliku JSON...");
        Debug.Log("Œcie¿ka StreamingAssets: " + Application.streamingAssetsPath);

        string path = File.Exists(PersistPath) ? PersistPath : StreamingPath;
        Debug.Log("U¿ywam œcie¿ki: " + path);
        Debug.Log("File.Exists? " + File.Exists(path));
        Debug.Log("Zawartoœæ StreamingAssets:\n" + string.Join("\n", Directory.GetFiles(Application.streamingAssetsPath)));

        if (!File.Exists(path))
        {
            Debug.LogError("NIE ZNALAZ£EM PLIKU: " + path);
            return;
        }

        try
        {
            var text = File.ReadAllText(path);
            Debug.Log($"Wczytano tekst JSON ({text.Length} znaków)");

            var root = JsonConvert.DeserializeObject<WorldFile>(text);
            if (root == null)
            {
                Debug.LogError("Deserializacja zwróci³a null!");
                return;
            }

            Data = root;
            Current = root.cities ?? new List<CityData>();
            After = root.after ?? null;

            // Jeœli istnieje sekcja "after", u¿yj jej jako bie¿¹cego stanu
            if (After != null && After.Count > 0)
            {
                Current = DeepClone(After);
                Debug.Log("U¿ywam danych z 'after' jako bie¿¹cego stanu œwiata");
            }

            Debug.Log($"Wczytano Current: {Current.Count} miast, After: {(After != null ? After.Count : 0)}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError("B³¹d parsowania JSON: " + ex.Message);
        }
    }

    public void SaveAfter()
    {
        // zaktualizuj „after” bie¿¹cym stanem i zapisz do persist
        Data.after = DeepClone(Current);
        string json = JsonConvert.SerializeObject(Data, Newtonsoft.Json.Formatting.Indented);
        File.WriteAllText(PersistPath, json);
    }

    public CityData FindCity(string name) => Current.Find(c => c.name == name);

    // proœciutki deep clone przez JSON — wystarczaj¹cy tutaj
    static T DeepClone<T>(T obj)
    {
        var json = JsonConvert.SerializeObject(obj);
        return JsonConvert.DeserializeObject<T>(json);
    }

    public bool AdjustCommodity(string city, string commodity, int deltaQty, int? newPrice = null)
    {
        var c = FindCity(city);
        if (c == null || c.commodities == null || !c.commodities.ContainsKey(commodity))
        { Debug.LogWarning($"Brak {city}/{commodity}"); return false; }

        var ce = c.commodities[commodity];
        ce.quantity = Mathf.Max(0, ce.quantity + deltaQty);
        if (newPrice.HasValue) ce.price = newPrice.Value;
        c.commodities[commodity] = ce;

        SaveAfter(); // zapisuje bie¿¹cy stan do sekcji "after" w pliku persist
        Debug.Log($"{city}:{commodity} ? qty={ce.quantity}, price={ce.price} (zapisano do AFTER)");
        return true;
    }

}


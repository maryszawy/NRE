using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

[Serializable]
public class EventDef
{
    [JsonProperty("name")] public string name;
    [JsonProperty("description")] public string description;
    [JsonProperty("target")] public string target;
    [JsonProperty("effects")] public JObject effects;
}

[Serializable]
public class EventsFile
{
    [JsonProperty("events")] public Dictionary<string, EventDef> events;
}

[Serializable]
public class CurrEventFile
{
    [JsonProperty("event_id")] public string event_id;
}

public class ZdarzeniaManager : MonoBehaviour
{
    public static ZdarzeniaManager Instance { get; private set; }

    private Dictionary<string, EventDef> _events = new Dictionary<string, EventDef>();
    private string _currEventId;

    [Header("UI")]
    public PanelZdarzenia panelZdarzenia;

    private string SciezkaEvents =>
        Path.Combine(Application.dataPath, "Data/Save/events_player.json");

    private string SciezkaCurrEvent =>
        Path.Combine(Application.dataPath, "Data/Save/curr_event_player.json");

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        ZaladujEvents();
        ZaladujCurrEvent();
    }

    private void ZaladujEvents()
    {
        try
        {
            if (!File.Exists(SciezkaEvents))
            {
                Debug.LogWarning("[ZdarzeniaManager] Brak events_player.json: " + SciezkaEvents);
                return;
            }

            var text = File.ReadAllText(SciezkaEvents);
            var root = JsonConvert.DeserializeObject<EventsFile>(text);
            _events = root?.events ?? new Dictionary<string, EventDef>();

            Debug.Log($"[ZdarzeniaManager] Wczytano events_player.json, liczba eventów: {_events.Count}");
        }
        catch (Exception ex)
        {
            Debug.LogError("[ZdarzeniaManager] B³¹d przy wczytywaniu events_player.json: " + ex.Message);
        }
    }

    private void ZaladujCurrEvent()
    {
        try
        {
            if (!File.Exists(SciezkaCurrEvent))
            {
                Debug.Log("[ZdarzeniaManager] Brak curr_event_player.json – brak zdarzenia do odpalenia.");
                _currEventId = null;
                return;
            }

            var text = File.ReadAllText(SciezkaCurrEvent);
            var root = JsonConvert.DeserializeObject<CurrEventFile>(text);
            _currEventId = root?.event_id;

            if (string.IsNullOrEmpty(_currEventId) || _currEventId == "null")
            {
                _currEventId = null;
                Debug.Log("[ZdarzeniaManager] curr_event_player.json ma null – brak zdarzenia.");
            }
            else
            {
                Debug.Log("[ZdarzeniaManager] curr_event_player.json -> event_id = " + _currEventId);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("[ZdarzeniaManager] B³¹d przy wczytywaniu curr_event_player.json: " + ex.Message);
            _currEventId = null;
        }
    }

    public bool CzyJestZdarzenieDoOdpalenia()
    {
        return !string.IsNullOrEmpty(_currEventId)
               && _events != null
               && _events.ContainsKey(_currEventId);
    }

    public void ObsluzZdarzeniePrzyPrzyjezdzie(Miasto miasto, Action onZakonczenie)
    {
        if (!CzyJestZdarzenieDoOdpalenia())
        {
            onZakonczenie?.Invoke();
            return;
        }

        var ev = _events[_currEventId];

        if (panelZdarzenia == null)
        {
            Debug.LogWarning("[ZdarzeniaManager] Brak panelZdarzenia – event bez UI, stosujê efekty od razu.");

            // awaryjnie: stosujemy efekty i czyœcimy event
            if (ev.target != null && ev.target.Equals("player", StringComparison.OrdinalIgnoreCase))
                ZastosujEfektyNaGraczu(ev);

            WyczyscCurrEvent();
            onZakonczenie?.Invoke();
            return;
        }

        // Teraz: najpierw pokazujemy panel, a dopiero PO klikniêciu OK stosujemy efekty
        panelZdarzenia.Pokaz(
            ev.name,
            ev.description,
            onClose: () =>
            {
                if (ev.target != null && ev.target.Equals("player", StringComparison.OrdinalIgnoreCase))
                {
                    ZastosujEfektyNaGraczu(ev);
                }
                else
                {
                    Debug.Log($"[ZdarzeniaManager] Event '{_currEventId}' target={ev.target} – pomijam efekty (na razie tylko player).");
                }

                WyczyscCurrEvent();
                onZakonczenie?.Invoke();
            }
        );
    }

    private void ZastosujEfektyNaGraczu(EventDef ev)
    {
        if (ev.effects == null)
            return;

        var stan = StanGracza.Instance;
        if (stan == null)
        {
            Debug.LogWarning("[ZdarzeniaManager] Brak StanGracza – nie mogê zastosowaæ eventu na graczu.");
            return;
        }

        var playerToken = ev.effects["player"] as JObject;
        if (playerToken == null)
            return;

        // z³oto
        int goldDelta = 0;
        if (playerToken["gold"] != null)
            goldDelta = playerToken.Value<int>("gold");
        else if (playerToken["zloto"] != null)
            goldDelta = playerToken.Value<int>("zloto");

        if (goldDelta != 0)
        {
            stan.DodajZloto(goldDelta);
            Debug.Log($"[ZdarzeniaManager] Event '{_currEventId}' zmienia z³oto o {goldDelta}.");
        }

        // ekwipunek
        var invToken = playerToken["inventory"] as JObject;
        if (invToken != null)
        {
            foreach (var prop in invToken.Properties())
            {
                string towar = prop.Name;
                int delta = prop.Value.Value<int>();
                stan.DodajTowar(towar, delta);
                Debug.Log($"[ZdarzeniaManager] Event '{_currEventId}' zmienia towar '{towar}' o {delta}.");
            }
        }
    }

    private void WyczyscCurrEvent()
    {
        _currEventId = null;

        try
        {
            var obj = new CurrEventFile { event_id = null };
            var json = JsonConvert.SerializeObject(obj, Formatting.Indented);
            File.WriteAllText(SciezkaCurrEvent, json);
            Debug.Log("[ZdarzeniaManager] Wyczyszczono curr_event_player.json");
        }
        catch (Exception ex)
        {
            Debug.LogError("[ZdarzeniaManager] B³¹d przy zapisie curr_event_player.json: " + ex.Message);
        }
    }
}

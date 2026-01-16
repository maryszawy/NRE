using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public class PanelRankingu : MonoBehaviour
{
    [Header("UI")]
    public GameObject kontenerGlowny;
    public Transform contentParent;
    public GameObject prefabWiersza;
    public bool Widoczny => kontenerGlowny != null && kontenerGlowny.activeSelf;

    private bool _widoczny = false;

    private class BotDataDTO
    {
        public string name;
        public float zloto;
    }
    private class PozycjaRankingu
    {
        public string Nazwa;
        public float Zloto;
        public bool ToGracz;
    }

    void Start()
    {
        if (kontenerGlowny) kontenerGlowny.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            PrzelaczWidok();
        }
    }

    public void PrzelaczWidok()
    {
        _widoczny = !_widoczny;
        kontenerGlowny.SetActive(_widoczny);

        if (_widoczny)
        {
            GenerujRanking();
        }
    }

    private void GenerujRanking()
    {
        foreach (Transform child in contentParent) Destroy(child.gameObject);

        List<PozycjaRankingu> lista = new List<PozycjaRankingu>();

        if (StanGracza.Instance != null)
        {
            lista.Add(new PozycjaRankingu
            {
                Nazwa = "Gracz",
                Zloto = StanGracza.Instance.dane.zloto,
                ToGracz = true
            });
        }

        string sciezkaSave = Path.Combine(Application.dataPath, "Data/Save");

        if (Directory.Exists(sciezkaSave))
        {
            string[] pliki = Directory.GetFiles(sciezkaSave, "bot*.json");

            foreach (string plik in pliki)
            {
                try
                {
                    string json = File.ReadAllText(plik);

                    BotDataDTO daneBota = JsonConvert.DeserializeObject<BotDataDTO>(json);

                    if (daneBota != null)
                    {
                        string wyswietlanaNazwa = !string.IsNullOrEmpty(daneBota.name)
                            ? daneBota.name
                            : Path.GetFileNameWithoutExtension(plik);

                        lista.Add(new PozycjaRankingu
                        {
                            Nazwa = wyswietlanaNazwa,
                            Zloto = daneBota.zloto,
                            ToGracz = false
                        });
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogWarning($"B³¹d odczytu bota {plik}: {ex.Message}");
                }
            }
        }
        else
        {
            Debug.LogWarning($"Folder nie istnieje: {sciezkaSave}");
        }

        lista.Sort((a, b) => b.Zloto.CompareTo(a.Zloto));

        for (int i = 0; i < lista.Count; i++)
        {
            GameObject go = Instantiate(prefabWiersza, contentParent);
            var wierszScript = go.GetComponent<WierszRankinguUI>();

            if (wierszScript != null)
            {
                wierszScript.Ustaw(i + 1, lista[i].Nazwa, lista[i].Zloto, lista[i].ToGracz);
            }
        }
    }
}
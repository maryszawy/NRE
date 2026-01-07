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
        // 1. Czyszczenie listy
        foreach (Transform child in contentParent) Destroy(child.gameObject);

        List<PozycjaRankingu> lista = new List<PozycjaRankingu>();

        // 2. Dodaj GRACZA (Stan aktualny z pamiêci)
        if (StanGracza.Instance != null)
        {
            lista.Add(new PozycjaRankingu
            {
                // Gracz w swoim JSON nie ma imienia, wiêc nadajemy je sztywno
                Nazwa = "TY (Gracz)",
                Zloto = StanGracza.Instance.dane.zloto,
                ToGracz = true
            });
        }

        // 3. Dodaj BOTY (Z plików JSON)
        // Œcie¿ka: Assets/Data/Save (dzia³a w Editorze)
        string sciezkaSave = Path.Combine(Application.dataPath, "Data/Save");

        if (Directory.Exists(sciezkaSave))
        {
            // Pobieramy wszystkie pliki zaczynaj¹ce siê od "bot"
            string[] pliki = Directory.GetFiles(sciezkaSave, "bot*.json");

            foreach (string plik in pliki)
            {
                try
                {
                    string json = File.ReadAllText(plik);

                    // U¿ywamy naszej pomocniczej klasy BotDataDTO
                    BotDataDTO daneBota = JsonConvert.DeserializeObject<BotDataDTO>(json);

                    if (daneBota != null)
                    {
                        // Jeœli w JSON jest "name", u¿yj go. Jeœli nie, u¿yj nazwy pliku.
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

        // 4. Sortowanie (Malej¹co po Z³ocie)
        lista.Sort((a, b) => b.Zloto.CompareTo(a.Zloto));

        // 5. Wyœwietlanie
        for (int i = 0; i < lista.Count; i++)
        {
            GameObject go = Instantiate(prefabWiersza, contentParent);
            var wierszScript = go.GetComponent<WierszRankinguUI>(); // Upewnij siê, ¿e masz ten skrypt (poni¿ej)

            if (wierszScript != null)
            {
                wierszScript.Ustaw(i + 1, lista[i].Nazwa, lista[i].Zloto, lista[i].ToGracz);
            }
        }
    }
}
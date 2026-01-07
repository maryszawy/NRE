using System.Collections.Generic;
using UnityEngine;

public class PanelRynkuSwiata : MonoBehaviour
{
    [Header("UI")]
    public GameObject kontenerGlowny;
    public Transform contentParent;
    public GameObject prefabWiersza;

    public bool Widoczny => kontenerGlowny != null && kontenerGlowny.activeSelf;

    [Header("Sterowanie")]
    public KeyCode klawisz = KeyCode.M;

    private bool _widoczny = false;

    void Start()
    {
        if (kontenerGlowny) kontenerGlowny.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(klawisz))
        {
            Przelacz();
        }
    }

    public void Przelacz()
    {
        _widoczny = !_widoczny;
        kontenerGlowny.SetActive(_widoczny);

        if (_widoczny)
        {
            OdswiezListe();
        }
    }

    private void OdswiezListe()
    {
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        if (RynekMiast.Instance == null || RynekMiast.Instance.Current == null)
        {
            Debug.LogWarning("[PanelRynkuSwiata] Brak danych miast!");
            return;
        }

        List<CityData> miasta = RynekMiast.Instance.Current;

        foreach (var miasto in miasta)
        {
            GameObject go = Instantiate(prefabWiersza, contentParent);
            var wierszUI = go.GetComponent<WierszMiastaUI>();

            if (wierszUI != null)
            {
                wierszUI.UstawDane(miasto);
            }
        }
    }
}
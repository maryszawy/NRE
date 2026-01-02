using TMPro;
using UnityEngine;

public class PanelEkwipunku : MonoBehaviour
{
    [Header("Referencje UI")]
    public GameObject kontener;
    public TextMeshProUGUI txtZloto;
    public TextMeshProUGUI txtMetal;
    public TextMeshProUGUI txtGems;
    public TextMeshProUGUI txtFood;
    public TextMeshProUGUI txtFuel;
    public TextMeshProUGUI txtRelics;
    public TextMeshProUGUI txtObciazenie;

    [Header("Sterowanie")]
    public KeyCode klawiszToggle = KeyCode.I;

    private void Start()
    {
        if (kontener) kontener.SetActive(false);

        // SUBSKRYPCJE (działają poprawnie)
        if (StanGracza.Instance != null)
        {
            StanGracza.Instance.OnZlotoZmiana += OnZlotoZmiana;
            StanGracza.Instance.OnEkwipunekZmiana += OnEkwipunekZmiana;
            StanGracza.Instance.OnObciazenieZmiana += OnObciazenieZmiana;
        }

        // odśwież dane od razu, żeby mieć aktualne wartości
        Odswiez();
    }

    private void OnDestroy()
    {
        if (StanGracza.Instance != null)
        {
            StanGracza.Instance.OnZlotoZmiana -= OnZlotoZmiana;
            StanGracza.Instance.OnEkwipunekZmiana -= OnEkwipunekZmiana;
            StanGracza.Instance.OnObciazenieZmiana -= OnObciazenieZmiana;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(klawiszToggle)) Przelacz();
    }

    public void Przelacz()
    {
        if (!kontener) return;

        bool nowe = !kontener.activeSelf;
        kontener.SetActive(nowe);

        if (nowe)
            Odswiez();
    }

    // --- OBSŁUGA EVENTÓW ---

    private void OnZlotoZmiana(int noweZloto)
    {
        Odswiez();
    }

    private void OnEkwipunekZmiana(string nazwaTowaru, int nowaIlosc)
    {
        Odswiez();
    }

    private void OnObciazenieZmiana(float aktualne, float max)
    {
        if (txtObciazenie != null)
            txtObciazenie.text = $"{aktualne:0.#} / {max:0.#}";
    }

    // --- ODŚWIEŻANIE UI ---

    public void Odswiez()
    {
        var sg = StanGracza.Instance;
        if (sg == null) return;

        txtZloto.text = $"Złoto: {sg.dane.zloto}";
        txtMetal.text = $"Metal: {sg.IleTowaru("metal")}";
        txtGems.text = $"Klejnoty: {sg.IleTowaru("gems")}";
        txtFood.text = $"Jedzenie: {sg.IleTowaru("food")}";
        txtFuel.text = $"Paliwo: {sg.IleTowaru("fuel")}";
        txtRelics.text = $"Relikty: {sg.IleTowaru("relics")}";
    }
}

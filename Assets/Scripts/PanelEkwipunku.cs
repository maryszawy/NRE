using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
        if (StanGracza.Instance != null)
            StanGracza.Instance.OnZlotoZmiana += _ => { if (kontener && kontener.activeSelf) Odswiez(); };
    }

    private void OnDestroy()
    {
        if (StanGracza.Instance != null)
            StanGracza.Instance.OnZlotoZmiana -= _ => { if (kontener && kontener.activeSelf) Odswiez(); };
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
        if (nowe) Odswiez();
    }

    public void Odswiez()
    {
        var sg = StanGracza.Instance;
        if (sg == null) return;

        txtZloto.text = $"Złoto: {sg.dane.zloto}";
        txtMetal.text = $"Metal: {sg.dane.ekwipunek["metal"]}";
        txtGems.text = $"Klejnoty: {sg.dane.ekwipunek["gems"]}";
        txtFood.text = $"Jedzenie: {sg.dane.ekwipunek["food"]}";
        txtFuel.text = $"Paliwo: {sg.dane.ekwipunek["fuel"]}";
        txtRelics.text = $"Relikty: {sg.dane.ekwipunek["relics"]}";
    }

    private void OnEnable()
    {
        if (StanGracza.Instance != null)
        {
            StanGracza.Instance.OnObciazenieZmiana += OnObciazenieZmiana;
            OnObciazenieZmiana(
                StanGracza.Instance.AktualneObciazenie,
                StanGracza.Instance.maksUdzwig);
        }
    }

    private void OnDisable()
    {
        if (StanGracza.Instance != null)
            StanGracza.Instance.OnObciazenieZmiana -= OnObciazenieZmiana;
    }

    void OnObciazenieZmiana(float aktualne, float max)
    {
        if (txtObciazenie != null)
        {
            txtObciazenie.text = $"{aktualne:0.#} / {max:0.#}";
        }
    }

}

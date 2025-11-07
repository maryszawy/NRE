using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PanelMiastoInfo : MonoBehaviour
{
    [Header("Referencje UI")]
    public GameObject kontener;
    public TextMeshProUGUI txtTytul;
    public Transform content;
    public GameObject prefabWiersz;
    public Button btnZamknij;

    private string _aktualneMiasto;

    private void Awake()
    {
        if (btnZamknij) btnZamknij.onClick.AddListener(Ukryj);
        Ukryj();
    }

    private void Update()
    {
        if (kontener.activeSelf && Input.GetKeyDown(KeyCode.Escape))
            Ukryj();
    }

    public void Pokaz(string nazwaMiasta)
    {
        _aktualneMiasto = nazwaMiasta;

        var miasto = RynekMiast.Instance?.FindCity(nazwaMiasta);
        if (miasto == null)
        {
            Debug.LogWarning($"[PanelMiastoInfo] Nie znaleziono miasta: {nazwaMiasta}");
            return;
        }

        if (txtTytul) txtTytul.text = $"{miasto.name}  —  op³ata: {miasto.fee}";

        foreach (Transform t in content) Destroy(t.gameObject);

        if (miasto.commodities != null)
        {
            var pary = miasto.commodities.Where(kv => kv.Key != "special");
            foreach (var kv in pary)
            {
                var go = Instantiate(prefabWiersz, content);
                var txty = go.GetComponentsInChildren<TextMeshProUGUI>(true);

                var txtNazwa = txty.FirstOrDefault(t => t.name.Contains("TxtNazwa")) ?? txty.FirstOrDefault();
                var txtOpis = txty.FirstOrDefault(t => t.name.Contains("TxtOpis")) ?? txty.LastOrDefault();

                if (txtNazwa) txtNazwa.text = kv.Key;
                if (txtOpis) txtOpis.text = $"iloœæ: {kv.Value.quantity} | cena: {kv.Value.price}";
            }
        }

        if (kontener) kontener.SetActive(true);
    }

    public void Ukryj()
    {
        if (kontener) kontener.SetActive(false);
    }
}

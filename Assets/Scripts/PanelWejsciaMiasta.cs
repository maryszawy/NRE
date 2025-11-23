using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PanelWejsciaMiasta : MonoBehaviour
{
    [Header("UI")]
    public GameObject root;
    public TextMeshProUGUI txtTytul;
    public TextMeshProUGUI txtOpis;
    public TextMeshProUGUI txtFee;
    public Button btnWejdz;
    public Button btnOdejdz;

    public bool Widoczny => root != null && root.activeSelf;

    private Action _poWejsciu;
    private Action _poOdejscie;

    private void Awake()
    {
        if (root == null) root = gameObject;

        if (btnWejdz) btnWejdz.onClick.AddListener(OnWejdzKlik);
        if (btnOdejdz) btnOdejdz.onClick.AddListener(OnOdejdzKlik);

        Ukryj();
    }

    public void Pokaz(string nazwaMiasta, int fee, Action poWejsciu, Action poOdejscie)
    {
        _poWejsciu = poWejsciu;
        _poOdejscie = poOdejscie;

        if (txtTytul)
            txtTytul.text = "Przed bram¹ miasta";

        if (txtOpis)
            txtOpis.text = $"Czy chcesz wejœæ do miasta <b>{nazwaMiasta}</b>?";

        if (txtFee)
            txtFee.text = $"Op³ata za wejœcie: <b>{fee}</b> z³ota";

        root.SetActive(true);
    }

    public void Ukryj()
    {
        if (root) root.SetActive(false);
    }

    private void OnWejdzKlik()
    {
        Ukryj();
        _poWejsciu?.Invoke();
    }

    private void OnOdejdzKlik()
    {
        Ukryj();
        _poOdejscie?.Invoke();
    }
}

using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PanelZdarzenia : MonoBehaviour
{
    [Header("Referencje UI")]
    [Tooltip("G³ówny obiekt panelu (overlay). Jeœli puste, u¿yje gameObject.")]
    public GameObject root;

    public TextMeshProUGUI txtTytul;
    public TextMeshProUGUI txtOpis;
    public Button btnOk;

    public bool Widoczny => (root ?? gameObject).activeSelf;

    private Action _onZamknieciu;

    private void Awake()
    {
        if (root == null)
            root = gameObject;

        root.SetActive(false);
    }
    
    public void Pokaz(string tytul, string opis, Action onClose)
    {
        _onZamknieciu = onClose;

        if (txtTytul != null) txtTytul.text = tytul;
        if (txtOpis != null) txtOpis.text = opis;

        root.SetActive(true);
    }

    public void OnKlikOk()
    {
        root.SetActive(false);

        _onZamknieciu?.Invoke();
        _onZamknieciu = null;
    }
}

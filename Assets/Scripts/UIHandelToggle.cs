using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIHandelToggle : MonoBehaviour
{
    [Header("Referencje")]
    public PanelMiastoInfo panelMiastoInfo;
    public GraczController gracz;
    public Button przycisk;

    [Header("Wygl¹d")]
    public Color kolorNormalny = Color.white;
    public Color kolorAktywny = new Color(1f, 0.85f, 0.4f);
    public Image tloIkony;

    private bool _aktywny;

    void Reset()
    {
        przycisk = GetComponent<Button>();
    }

    void Awake()
    {
        if (!przycisk) przycisk = GetComponent<Button>();
        przycisk.onClick.AddListener(OnKlik);
    }

    void OnDestroy()
    {
        if (przycisk) przycisk.onClick.RemoveListener(OnKlik);
    }

    void Update()
    {
        if (gracz)
            przycisk.interactable = !gracz.czyWTrasie;

        if (Input.GetKeyDown(KeyCode.H))
        {
            var go = EventSystem.current ? EventSystem.current.currentSelectedGameObject : null;
            if (go == null || go.GetComponent<TMPro.TMP_InputField>() == null)
            {
                ToggleHandel();
            }
        }
    }

    private void OnKlik()
    {
        ToggleHandel();
    }

    private void ToggleHandel()
    {
        if (!panelMiastoInfo || !gracz) return;
        if (gracz.czyWTrasie) return;

        _aktywny = !_aktywny;

        if (_aktywny)
        {
            var miasto = gracz.AktualneMiasto;
            if (miasto != null)
                panelMiastoInfo.Pokaz(miasto.nazwa);
        }
        else
        {
            panelMiastoInfo.Ukryj();
        }

        if (tloIkony)
            tloIkony.color = _aktywny ? kolorAktywny : kolorNormalny;
    }
}

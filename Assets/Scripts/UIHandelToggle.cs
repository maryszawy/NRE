using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIHandelToggle : MonoBehaviour
{
    [Header("Referencje")]
    public PanelMiastoInfo panelMiastoInfo;
    public GraczController gracz;
    public Button przycisk;
    public Image tloIkony;

    [Header("Wygl¹d")]
    public Color kolorNormalny = Color.white;
    public Color kolorAktywny = new Color(1f, 0.85f, 0.4f);

    private bool _aktywny;

    void Reset()
    {
        przycisk = GetComponent<Button>();
    }

    void Awake()
    {
        if (!przycisk) przycisk = GetComponent<Button>();
        przycisk.onClick.AddListener(OnKlik);
        UstawKolor(false);
    }

    void OnDestroy()
    {
        if (przycisk) przycisk.onClick.RemoveListener(OnKlik);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            if (przycisk != null && przycisk.interactable && CzyMoznaHandlowac())
            {
                ToggleHandel();
            }
        }
    }

    private bool CzyMoznaHandlowac()
    {
        if (MapaGry.Instance == null) return false;
        if (gracz != null && gracz.czyWTrasie) return false;
        return MapaGry.Instance.moznaHandlowac;
    }

    private void OnKlik()
    {
        if (!przycisk.interactable) return;
        if (!CzyMoznaHandlowac()) return;

        ToggleHandel();
    }

    private void ToggleHandel()
    {
        if (panelMiastoInfo == null || gracz == null) return;

        if (!_aktywny)
        {
            var miasto = gracz.AktualneMiasto;
            if (miasto == null) return;

            panelMiastoInfo.Pokaz(miasto.nazwa);
            _aktywny = true;
        }
        else
        {
            panelMiastoInfo.Ukryj();
            _aktywny = false;
        }

        UstawKolor(_aktywny);
    }

    private void UstawKolor(bool aktywny)
    {
        if (tloIkony != null)
            tloIkony.color = aktywny ? kolorAktywny : kolorNormalny;
    }
}

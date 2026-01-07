using UnityEngine;

public class MenadzerUI : MonoBehaviour
{
    public static MenadzerUI Instance;

    [Header("Panele Blokuj¹ce")]
    public PanelMiastoInfo panelMiasta;
    public PanelRankingu panelRankingu;
    public PanelWejsciaMiasta panelWejsciaMiasta;
    public PanelRynkuSwiata panelRynkuSwiata;
    public PanelPotwierdzenia panelPotwierdzenia;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    public bool CzyUIBlokuje
    {
        get
        {
            if (panelMiasta != null && panelMiasta.Widoczny) return true;
            if (panelRankingu != null && panelRankingu.Widoczny) return true;
            if (panelWejsciaMiasta != null && panelWejsciaMiasta.Widoczny) return true;
            if (panelRynkuSwiata != null && panelRynkuSwiata.Widoczny) return true;
            if (panelPotwierdzenia != null && panelPotwierdzenia.Widoczny) return true;

            if (Time.timeScale == 0) return true;

            return false;
        }
    }
}
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuGlowneUI : MonoBehaviour
{
    [Header("Sceny")]
    [Tooltip("Nazwa sceny z map¹ / w³aœciw¹ gr¹ (np. SampleScene).")]
    public string nazwaScenyGry = "SampleScene";

    [Header("Przyciski")]
    public Button btnWczytaj;

    private void Start()
    {
        // jeœli nie ma zapisu gracza, to przycisk Wczytaj wy³¹czony
        if (btnWczytaj != null)
        {
            bool jestSave = File.Exists(SciezkiZapisu.PlikGracza);
            btnWczytaj.interactable = jestSave;
        }
    }

    // === OnClick dla przycisku "Nowa gra" ===
    public void OnKlikNowaGra()
    {
        // 1) wyczyœæ istniej¹ce save'y
        ResetujZapisy();

        // 2) za³aduj scenê z gr¹
        SceneManager.LoadScene(nazwaScenyGry);
    }

    // === OnClick dla przycisku "Wczytaj" ===
    public void OnKlikWczytaj()
    {
        // po prostu ³adujemy scenê – StanGracza / RynekMiast wczytaj¹ dane same w Awake/Wczytaj()
        SceneManager.LoadScene(nazwaScenyGry);
    }

    // === OnClick dla przycisku "WyjdŸ" ===
    public void OnKlikWyjdz()
    {
        Debug.Log("[MenuGlowneUI] Wyjœcie z gry");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void ResetujZapisy()
    {
        // gracz
        if (File.Exists(SciezkiZapisu.PlikGracza))
            File.Delete(SciezkiZapisu.PlikGracza);

        // miasta
        if (File.Exists(SciezkiZapisu.PlikMiast))
            File.Delete(SciezkiZapisu.PlikMiast);

        // eventy gracza (curr_event_player)
        string sciezkaCurrEvent = Path.Combine(Application.dataPath, "Data/Save/curr_event_player.json");
        if (File.Exists(sciezkaCurrEvent))
            File.Delete(sciezkaCurrEvent);

        Debug.Log("[MenuGlowneUI] Usuniêto stare save'y (nowa gra).");
    }
}

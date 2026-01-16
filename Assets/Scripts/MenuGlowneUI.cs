using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuGlowneUI : MonoBehaviour
{
    [Header("Sceny")]
    [Tooltip("Nazwa sceny z mapa / w³asciwa gra (np. SampleScene).")]
    public string nazwaScenyGry = "SampleScene";

    [Header("Przyciski")]
    public Button btnWczytaj;

    private void Start()
    {
        if (btnWczytaj != null)
        {
            bool jestSave = File.Exists(SciezkiZapisu.PlikGracza);
            btnWczytaj.interactable = jestSave;
        }
    }

    public void OnKlikNowaGra()
    {
        ResetujZapisy();

        SceneManager.LoadScene(nazwaScenyGry);
    }

    public void OnKlikWczytaj()
    {
        SceneManager.LoadScene(nazwaScenyGry);
    }

    public void OnKlikWyjdz()
    {
        Debug.Log("[MenuGlowneUI] Wyjscie z gry");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void ResetujZapisy()
    {
        if (File.Exists(SciezkiZapisu.PlikGracza))
            File.Delete(SciezkiZapisu.PlikGracza);

        if (File.Exists(SciezkiZapisu.PlikMiast))
            File.Delete(SciezkiZapisu.PlikMiast);

        string sciezkaCurrEvent = Path.Combine(Application.dataPath, "Data/Save/curr_event_player.json");
        if (File.Exists(sciezkaCurrEvent))
            File.Delete(sciezkaCurrEvent);

        Debug.Log("[MenuGlowneUI] Usunieto stare save'y (nowa gra).");
    }
}

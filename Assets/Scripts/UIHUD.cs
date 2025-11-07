using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIHUD : MonoBehaviour
{
    [Header("Referencje UI")]
    public TextMeshProUGUI txtCzas;
    public TextMeshProUGUI txtPodroz;
    public Image pasekPostepu;

    [Header("Źródła danych")]
    public GraczController gracz;

    private float minutyGryCalkowite = 0f;
    private string nazwaStart = "";
    private string nazwaCel = "";

    private void Start()
    {
        if (gracz == null) gracz = FindObjectOfType<GraczController>();

        if (CzasGry.Instance != null)
        {
            CzasGry.Instance.OnZmianaCzasu += OdwiezCzas;
            OdwiezCzas();
        }

        UkryjPodroz();

        if (gracz != null)
        {
            gracz.OnPodrozStart += OnPodrozStart;
            gracz.OnPodrozPostep += OnPodrozPostep;
            gracz.OnPodrozKoniec += OnPodrozKoniec;
        }
    }

    private void OnDestroy()
    {
        if (CzasGry.Instance != null) CzasGry.Instance.OnZmianaCzasu -= OdwiezCzas;
        if (gracz != null)
        {
            gracz.OnPodrozStart -= OnPodrozStart;
            gracz.OnPodrozPostep -= OnPodrozPostep;
            gracz.OnPodrozKoniec -= OnPodrozKoniec;
        }
    }

    private void OdwiezCzas()
    {
        if (txtCzas != null && CzasGry.Instance != null)
            txtCzas.text = CzasGry.Instance.FormatujDzienIGodzine();
    }

    private void UkryjPodroz()
    {
        if (txtPodroz) txtPodroz.text = "";
        if (pasekPostepu) pasekPostepu.fillAmount = 0f;
    }

    private void OnPodrozStart(GraczController.InfoPodrozy info)
    {
        minutyGryCalkowite = info.minutyGryCalkowite;
        nazwaStart = info.nazwaStart;
        nazwaCel = info.nazwaCel;

        if (txtPodroz)
            txtPodroz.text = $"Travel: {nazwaStart} -> {nazwaCel} | RTT: {FormatujMinuty(info.minutyGryCalkowite)}";

        if (pasekPostepu) pasekPostepu.fillAmount = 0f;
    }

    private void OnPodrozPostep(float progres0do1)
    {
        if (pasekPostepu)
        {
            pasekPostepu.fillAmount = Mathf.Clamp01(progres0do1);
            pasekPostepu.color = Color.Lerp(Color.red, Color.green, progres0do1);
        }

        float pozostale = Mathf.Max(0f, minutyGryCalkowite * (1f - progres0do1));
        if (txtPodroz)
            txtPodroz.text = $"Travel: {nazwaStart} -> {nazwaCel} | RTT: {FormatujMinuty(pozostale)}";
    }


    private void OnPodrozKoniec()
    {
        if (txtPodroz)
            txtPodroz.text = $"Reached: {nazwaCel}";
        if (pasekPostepu) pasekPostepu.fillAmount = 1f;
    }

    private string FormatujMinuty(float min)
    {
        int m = Mathf.RoundToInt(min);
        int h = m / 60;
        int mm = m % 60;
        if (h > 0) return $"{h}h {mm}m";
        return $"{mm}m";
    }
}

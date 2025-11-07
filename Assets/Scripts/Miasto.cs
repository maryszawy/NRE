using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
public class Miasto : MonoBehaviour
{
    [Header("Dane")]
    public string nazwa = "Miasto";
    public List<Droga> drogi = new List<Droga>();

    [Header("Wygl¹d")]
    public Color kolorNormalny = Color.white;
    public Color kolorZaznaczenia = new Color(1f, 0.85f, 0.2f);

    [Header("Hover")]
    public float skalowanieHover = 1.08f;
    public Color kolorHover = new Color(1f, 1f, 1f, 1f);
    public float czasPrzejscia = 0.08f;

    private SpriteRenderer _sr;
    private Vector3 _skalaBazowa;
    private Color _kolorBazowy;
    private bool _hover;

    void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        if (_sr != null) _kolorBazowy = _sr.color;
        _skalaBazowa = transform.localScale;
    }

    void OnMouseEnter()
    {
        _hover = true;
        StopAllCoroutines();
        StartCoroutine(AnimujHover(_skalaBazowa * skalowanieHover, kolorHover));
        TooltipMiasto.Instance?.Pokaz(nazwa);
    }

    void OnMouseExit()
    {
        _hover = false;
        StopAllCoroutines();
        StartCoroutine(AnimujHover(_skalaBazowa, _kolorBazowy));
        TooltipMiasto.Instance?.Ukryj();
    }

    System.Collections.IEnumerator AnimujHover(Vector3 docelowaSkala, Color docelowyKolor)
    {
        float t = 0f;
        Vector3 startS = transform.localScale;
        Color startC = (_sr != null ? _sr.color : Color.white);

        while (t < czasPrzejscia)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / Mathf.Max(0.0001f, czasPrzejscia));
            transform.localScale = Vector3.Lerp(startS, docelowaSkala, k);
            if (_sr != null) _sr.color = Color.Lerp(startC, docelowyKolor, k);
            yield return null;
        }

        transform.localScale = docelowaSkala;
        if (_sr != null) _sr.color = docelowyKolor;
    }

    public void UstawZaznaczenie(bool wlacz)
    {
        if (_sr != null) _sr.color = wlacz ? kolorZaznaczenia : kolorNormalny;
    }

    void OnMouseDown()
    {
        MapaGry.Instance?.KliknietoMiasto(this);
    }
}

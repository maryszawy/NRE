using TMPro;
using UnityEngine;

public class TooltipMiasto : MonoBehaviour
{
    public static TooltipMiasto Instance { get; private set; }

    [Header("UI")]
    public RectTransform kontener;
    public TextMeshProUGUI txtNazwa;

    [Header("Pozycjonowanie")]
    public Vector2 przesuniecie = new Vector2(16f, 20f);
    public bool autoClamp = true;

    Canvas _canvas;
    bool _widoczny;

    void Awake()
    {
        Instance = this;
        _canvas = GetComponentInParent<Canvas>();
        if (!kontener) kontener = GetComponent<RectTransform>();
        Ukryj();
    }

    void Update()
    {
        if (_widoczny) UstawPrzyMyszce();
    }

    public void Pokaz(string nazwa)
    {
        if (txtNazwa) txtNazwa.text = nazwa;
        if (!gameObject.activeSelf) gameObject.SetActive(true);
        _widoczny = true;
        UstawPrzyMyszce();
    }

    public void Ukryj()
    {
        _widoczny = false;
        if (gameObject.activeSelf) gameObject.SetActive(false);
    }

    void UstawPrzyMyszce()
    {
        Vector2 cel = (Vector2)Input.mousePosition + przesuniecie;

        if (autoClamp && _canvas != null)
        {
            var rect = _canvas.pixelRect;
            var w = kontener.rect.width;
            var h = kontener.rect.height;
            cel.x = Mathf.Clamp(cel.x, w * 0.5f, rect.width - w * 0.5f);
            cel.y = Mathf.Clamp(cel.y, h * 0.5f, rect.height - h * 0.5f);
        }

        kontener.position = cel;
    }
}

using UnityEngine;

[RequireComponent(typeof(Camera))]
public class KameraMapa : MonoBehaviour
{
    [Header("Œledzenie gracza")]
    public Transform gracz;   
    public bool sledzGracza = true; 
    public float gladkosc = 5f;  
    public Vector3 przesuniecie = new Vector3(0, 0, -10);

    [Header("Sterowanie rêczne (opcjonalne)")]
    public float predkoscPanu = 20f;
    public float zoomMin = 2f;
    public float zoomMax = 20f;
    public float czuloscZoom = 5f;

    private Camera _cam;
    private Bounds _graniceMapy;
    private bool _maGranice;

    private Vector3 _ostatniaPozycjaMyszy;

    private void Awake()
    {
        _cam = GetComponent<Camera>();
    }

    public void SkonfigurujGranice(Bounds granice)
    {
        _graniceMapy = granice;
        _maGranice = true;
        OgraniczWKadrze();
    }

    private void LateUpdate()
    {
        ObsluzZoom();

        if (sledzGracza && gracz != null)
        {
            Vector3 cel = gracz.position + przesuniecie;
            cel.z = transform.position.z;
            transform.position = Vector3.Lerp(transform.position, cel, gladkosc * Time.deltaTime);
        }
        else
        {
            ObsluzPan();
        }

        OgraniczWKadrze();
    }

    private void ObsluzZoom()
    {
        float scroll = Input.mouseScrollDelta.y;

        if (Mathf.Approximately(scroll, 0f))
            scroll = Input.GetAxis("Mouse ScrollWheel");

        if (Mathf.Abs(scroll) > 0.0001f)
        {
            _cam.orthographicSize = Mathf.Clamp(
                _cam.orthographicSize - scroll * czuloscZoom,
                zoomMin, zoomMax
            );
        }
    }

    private void ObsluzPan()
    {
        if (Input.GetMouseButtonDown(1))
            _ostatniaPozycjaMyszy = Input.mousePosition;

        if (Input.GetMouseButton(1))
        {
            var delta = Input.mousePosition - _ostatniaPozycjaMyszy;
            _ostatniaPozycjaMyszy = Input.mousePosition;

            var ruch = new Vector3(-delta.x, -delta.y, 0f) * (_cam.orthographicSize / 300f);
            transform.position += ruch;
        }
    }

    private void OgraniczWKadrze()
    {
        if (!_maGranice) return;

        float camH = _cam.orthographicSize;
        float camW = camH * _cam.aspect;

        var min = _graniceMapy.min;
        var max = _graniceMapy.max;

        float clampX = Mathf.Clamp(transform.position.x, min.x + camW, max.x - camW);
        float clampY = Mathf.Clamp(transform.position.y, min.y + camH, max.y - camH);

        transform.position = new Vector3(clampX, clampY, transform.position.z);
    }
}

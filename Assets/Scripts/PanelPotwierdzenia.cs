using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class PanelPotwierdzenia : MonoBehaviour
{
    [Header("UI")]
    public GameObject kontener;
    public TextMeshProUGUI tekstTytul;
    public TextMeshProUGUI tekstOpis;
    public TextMeshProUGUI tekstETA;
    public Button przyciskTak;
    public Button przyciskNie;

    [Header("Dźwięki (SFX)")]
    public AudioSource uiAudioSource;  
    public AudioClip dzwiekOtworz;
    public AudioClip dzwiekHover;
    public AudioClip dzwiekPotwierdz;
    public AudioClip dzwiekAnuluj;

    [Header("Sterowanie klawiaturą")]
    public bool sterowanieKlawiatura = true;

    private Action _poPotwierdzeniu;
    private Action _poAnulowaniu;

    private GameObject _ostatnioWybrany;

    private void Awake()
    {
        Ukryj();

        if (przyciskTak) przyciskTak.onClick.AddListener(Potwierdz);
        if (przyciskNie) przyciskNie.onClick.AddListener(Anuluj);


        if (przyciskTak && przyciskNie)
        {
            var navTak = new Navigation { mode = Navigation.Mode.Explicit };
            navTak.selectOnLeft = navTak.selectOnRight = navTak.selectOnUp = navTak.selectOnDown = przyciskNie;
            przyciskTak.navigation = navTak;

            var navNie = new Navigation { mode = Navigation.Mode.Explicit };
            navNie.selectOnLeft = navNie.selectOnRight = navNie.selectOnUp = navNie.selectOnDown = przyciskTak;
            przyciskNie.navigation = navNie;
        }
    }

    public bool Widoczny => gameObject != null && gameObject.activeSelf;

    public void Pokaz(string nazwaStart, string nazwaCel, string tekstCzasu,
                      Action poPotwierdzeniu, Action poAnulowaniu)
    {
        _poPotwierdzeniu = poPotwierdzeniu;
        _poAnulowaniu = poAnulowaniu;

        if (tekstTytul) tekstTytul.text = $"Czy chcesz jechać do: {nazwaCel}?";
        if (tekstOpis) tekstOpis.text = $"Z: {nazwaStart} → Do: {nazwaCel}\nPotwierdzenie podróży zakończy turę handlu.";
        if (tekstETA) tekstETA.text = $"Czas podróży: {tekstCzasu}";

        kontener.SetActive(true);
        Time.timeScale = 0f;

        ZagrajSFX(dzwiekOtworz);

        StartCoroutine(UstawFocusZaChwile());
        _ostatnioWybrany = null; 
    }

    public void Ukryj()
    {
        kontener.SetActive(false);
        Time.timeScale = 1f;

        _poPotwierdzeniu = null;
        _poAnulowaniu = null;
        _ostatnioWybrany = null;
    }

    private IEnumerator UstawFocusZaChwile()
    {
        yield return null;
        if (przyciskTak)
        {
            przyciskTak.Select();
            EventSystem.current?.SetSelectedGameObject(przyciskTak.gameObject);
            _ostatnioWybrany = przyciskTak.gameObject;
        }
    }

    private void Update()
    {
        if (!sterowanieKlawiatura || !kontener.activeSelf) return;

        if (WcisnietoEnter()) Potwierdz();
        if (WcisnietoEsc()) Anuluj();

        var sel = EventSystem.current ? EventSystem.current.currentSelectedGameObject : null;
        if (sel != null && sel != _ostatnioWybrany)
        {
            if (sel == przyciskTak?.gameObject || sel == przyciskNie?.gameObject)
                ZagrajSFX(dzwiekHover);
            _ostatnioWybrany = sel;
        }
    }

    private void Potwierdz() => StartCoroutine(ZamknijPoSFX(dzwiekPotwierdz, _poPotwierdzeniu));
    private void Anuluj() => StartCoroutine(ZamknijPoSFX(dzwiekAnuluj, _poAnulowaniu));

    private IEnumerator ZamknijPoSFX(AudioClip clip, Action callback)
    {
        ZagrajSFX(clip);
        yield return null;

        callback?.Invoke();
        Ukryj();
    }

    private void ZagrajSFX(AudioClip clip)
    {
        if (clip == null) return;

        if (uiAudioSource != null && uiAudioSource.gameObject.activeInHierarchy && uiAudioSource.enabled)
        {
            uiAudioSource.PlayOneShot(clip);
        }
        else
        {
            var pos = Camera.main ? Camera.main.transform.position : Vector3.zero;
            AudioSource.PlayClipAtPoint(clip, pos);
        }
    }

    private bool WcisnietoEnter()
    {
#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null &&
            (Keyboard.current.enterKey.wasPressedThisFrame || Keyboard.current.numpadEnterKey.wasPressedThisFrame))
            return true;
#endif
        return Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter);
    }

    private bool WcisnietoEsc()
    {
#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            return true;
#endif
        return Input.GetKeyDown(KeyCode.Escape);
    }
}

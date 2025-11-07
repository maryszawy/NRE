using UnityEngine;
using UnityEngine.EventSystems;

public class UIHoverSfx : MonoBehaviour, IPointerEnterHandler, ISelectHandler
{
    public AudioSource uiAudioSource;
    public AudioClip hoverClip;

    public void OnPointerEnter(PointerEventData eventData)
    {
        Zagraj();
    }

    public void OnSelect(BaseEventData eventData)
    {
        Zagraj();
    }

    private void Zagraj()
    {
        if (hoverClip == null) return;

        if (uiAudioSource != null && uiAudioSource.enabled && uiAudioSource.gameObject.activeInHierarchy)
            uiAudioSource.PlayOneShot(hoverClip);
        else
            AudioSource.PlayClipAtPoint(hoverClip, Camera.main ? Camera.main.transform.position : Vector3.zero);
    }
}

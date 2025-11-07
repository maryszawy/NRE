using TMPro;
using UnityEngine;

public class UIWaluta : MonoBehaviour
{
    public TextMeshProUGUI txtZloto;

    private void Start()
    {
        if (StanGracza.Instance != null)
        {
            StanGracza.Instance.OnZlotoZmiana += AktualizujUI;
            AktualizujUI(StanGracza.Instance.dane.zloto);
        }
    }

    private void OnDestroy()
    {
        if (StanGracza.Instance != null)
            StanGracza.Instance.OnZlotoZmiana -= AktualizujUI;
    }

    private void AktualizujUI(int nowaWartosc)
    {
        txtZloto.text = $"{nowaWartosc}";
    }
}

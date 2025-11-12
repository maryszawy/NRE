using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WierszTowaruUI : MonoBehaviour
{
    public TextMeshProUGUI txtNazwa;
    public TextMeshProUGUI txtCena;
    public TextMeshProUGUI txtMiastoIlosc;
    public TextMeshProUGUI txtGraczIlosc;
    public Button btnKup;
    public Button btnSprzedaj;

    [HideInInspector] public string nazwaTowaru;
}
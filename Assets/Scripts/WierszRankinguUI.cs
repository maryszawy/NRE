using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class WierszRankinguUI : MonoBehaviour
{
    public TextMeshProUGUI txtPozycja;
    public TextMeshProUGUI txtNazwa;
    public TextMeshProUGUI txtZloto;

    public Image tloWiersza;

    public void Ustaw(int pozycja, string nazwa, float zloto, bool toGracz)
    {
        txtPozycja.text = $"{pozycja}.";
        txtNazwa.text = nazwa;

        txtZloto.text = $"{zloto:F2}";

        if (toGracz)
        {
            txtNazwa.color = new Color(1f, 0.8f, 0.2f);
            txtZloto.color = new Color(1f, 0.8f, 0.2f);
            if (tloWiersza) tloWiersza.color = new Color(1f, 1f, 1f, 0.1f);
        }
        else
        {
            txtNazwa.color = Color.white;
            txtZloto.color = Color.white;
            if (tloWiersza) tloWiersza.color = new Color(0f, 0f, 0f, 0.5f);
        }
    }
}
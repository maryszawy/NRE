using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class WierszRankinguUI : MonoBehaviour
{
    public TextMeshProUGUI txtPozycja;
    public TextMeshProUGUI txtNazwa;
    public TextMeshProUGUI txtZloto;

    // Opcjonalnie: Obrazek t³a, ¿eby wyró¿niæ gracza
    public Image tloWiersza;

    public void Ustaw(int pozycja, string nazwa, float zloto, bool toGracz)
    {
        txtPozycja.text = $"{pozycja}.";
        txtNazwa.text = nazwa;

        // F2 = dwa miejsca po przecinku (np. 14176.39)
        txtZloto.text = $"{zloto:F2}";

        // Opcjonalne: Kolorowanie gracza na z³oto
        if (toGracz)
        {
            txtNazwa.color = new Color(1f, 0.8f, 0.2f); // Z³oty
            txtZloto.color = new Color(1f, 0.8f, 0.2f);
            if (tloWiersza) tloWiersza.color = new Color(1f, 1f, 1f, 0.1f); // Lekkie podœwietlenie t³a
        }
        else
        {
            txtNazwa.color = Color.white;
            txtZloto.color = Color.white;
            if (tloWiersza) tloWiersza.color = new Color(0f, 0f, 0f, 0.5f); // Ciemne t³o
        }
    }
}
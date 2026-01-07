using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class WierszMiastaUI : MonoBehaviour
{
    [Header("Info o Mieœcie")]
    public TextMeshProUGUI txtNazwaMiasta;
    public TextMeshProUGUI txtOplata;

    [Header("Kolumny Towarów (Ceny)")]
    public TextMeshProUGUI txtMetal;
    public TextMeshProUGUI txtGems;
    public TextMeshProUGUI txtFood;
    public TextMeshProUGUI txtFuel;
    public TextMeshProUGUI txtRelics;

    public void UstawDane(CityData miasto)
    {
        if (miasto == null) return;

        txtNazwaMiasta.text = miasto.name;
        txtOplata.text = $"{miasto.fee}";

        UstawCene(txtMetal, miasto, GameIDs.Metal);
        UstawCene(txtGems, miasto, GameIDs.Gems);
        UstawCene(txtFood, miasto, GameIDs.Food);
        UstawCene(txtFuel, miasto, GameIDs.Fuel);
        UstawCene(txtRelics, miasto, GameIDs.Relics);
    }

    private void UstawCene(TextMeshProUGUI txt, CityData miasto, string towarID)
    {
        if (txt == null) return;

        if (miasto.commodities != null && miasto.commodities.TryGetValue(towarID, out var towar))
        {
            float cenaKupna = RynekMiast.ObliczCeneKupna(miasto, towarID);
            float ilosc = towar.quantity;

            txt.text = $"{cenaKupna:F2}";

            float ratio = RynekMiast.GetStockRatio(miasto, towarID);

            if (ratio >= 1.0f)
                txt.color = Color.green;
            else if (ratio < 0.2f)
                txt.color = new Color(1f, 0.4f, 0.4f);
            else
                txt.color = Color.white;
        }
        else
        {
            txt.text = "-";
            txt.color = Color.gray;
        }
    }
}
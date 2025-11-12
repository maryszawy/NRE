using System.Collections.Generic;
using UnityEngine;

public partial class RynekMiast : MonoBehaviour
{
    static readonly Dictionary<string, string> _fabrykaDlaTowaru = new()
    {
        {"metal","mine"},
        {"gems","mine"},
        {"food","farm"},
        {"fuel","refinery"},
        {"relics","excavation_site"},
    };

    public static float GetStockRatio(CityData miasto, string towar)
    {
        if (miasto == null || miasto.commodities == null) return 1f;
        if (!miasto.commodities.TryGetValue(towar, out var c) || c == null) return 1f;
        float rq = Mathf.Max(1, c.regular_quantity);
        return Mathf.Clamp01(c.quantity / rq);
    }

    public static float Lerp(float a, float b, float t) => a + (b - a) * Mathf.Clamp01(t);

    public static float GetSellMultiplier(CityData miasto, string towar)
    {
        float r = GetStockRatio(miasto, towar);
        float mult;

        if (r < 0.10f) mult = Lerp(0.90f, 0.80f, r / 0.10f);          // prawie brak -> du¿y popyt
        else if (r < 0.40f) mult = Lerp(0.75f, 0.60f, (r - 0.10f) / 0.30f);
        else if (r <= 0.80f) mult = Lerp(0.50f, 0.30f, (r - 0.40f) / 0.40f);
        else mult = Lerp(0.25f, 0.00f, (r - 0.80f) / 0.20f);  // du¿o -> prawie nic nie p³ac¹

        // bonusy od fabryk
        if (_fabrykaDlaTowaru.TryGetValue(towar, out var fab) && !string.IsNullOrEmpty(fab))
        {
            if (miasto.factory != null && miasto.factory.Contains(fab))
                mult = Mathf.Min(0.95f, mult + 0.05f);
        }

        return Mathf.Clamp01(mult);
    }

    public static int ObliczCeneKupna(CityData miasto, string towar)
    {
        // kupno = 100% ceny bazowej miasta
        return (miasto != null && miasto.commodities != null && miasto.commodities.TryGetValue(towar, out var c)) ? c.price : 0;
    }

    public static int ObliczWyplateSprzedazy(CityData miasto, string towar)
    {
        if (miasto == null || miasto.commodities == null || !miasto.commodities.TryGetValue(towar, out var c))
            return 0;
        float mult = GetSellMultiplier(miasto, towar);
        return Mathf.RoundToInt(c.price * mult);
    }
}

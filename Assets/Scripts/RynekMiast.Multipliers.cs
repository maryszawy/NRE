using System.Collections.Generic;
using UnityEngine;

public partial class RynekMiast : MonoBehaviour
{
    static readonly Dictionary<string, string> _fabrykaDlaTowaru = new()
    {
        { GameIDs.Metal,  "mine" },
        { GameIDs.Gems,   "mine" },
        { GameIDs.Food,   "farm" },
        { GameIDs.Fuel,   "refinery" },
        { GameIDs.Relics, "excavation_site" },
    };

    public static float GetStockRatio(CityData miasto, string towar)
    {
        if (miasto == null || miasto.commodities == null) return 1f;
        if (!miasto.commodities.TryGetValue(towar, out var c) || c == null) return 1f;

        float rq = Mathf.Max(1, c.regular_quantity);
        return c.quantity / rq;
    }

    public static float Lerp(float a, float b, float t) => a + (b - a) * Mathf.Clamp01(t);

    public static float GetSellMultiplier(CityData miasto, string towar)
    {
        float r = GetStockRatio(miasto, towar);
        float mult;

        if (r < 0.2f)       // 0% - 20%: Bardzo du¿y popyt
            mult = Lerp(1.50f, 1.10f, r / 0.2f);

        else if (r < 0.5f)  // 20% - 50%: Normalny popyt
            mult = Lerp(1.10f, 0.90f, (r - 0.2f) / 0.3f);

        else if (r < 1.0f)  // 50% - 100%: Nasycanie rynku
            mult = Lerp(0.90f, 0.50f, (r - 0.5f) / 0.5f);

        else                // > 100%: Nadprodukcja (cena spada powoli do 10%)
            mult = Mathf.Max(0.1f, 0.50f - (r - 1.0f) * 0.2f);

        if (_fabrykaDlaTowaru.TryGetValue(towar, out var fab) && !string.IsNullOrEmpty(fab))
        {
            if (miasto.factory != null && miasto.factory.Contains(fab))
                mult += 0.10f;
        }

        return Mathf.Max(0.05f, mult);
    }

    public static int ObliczCeneKupna(CityData miasto, string towar)
    {
        if (miasto == null || miasto.commodities == null || !miasto.commodities.TryGetValue(towar, out var c))
            return 0;

        float sellMult = GetSellMultiplier(miasto, towar);

        float buyMult = sellMult * 1.15f;

        return Mathf.RoundToInt(c.price * buyMult);
    }

    public static int ObliczWyplateSprzedazy(CityData miasto, string towar)
    {
        if (miasto == null || miasto.commodities == null || !miasto.commodities.TryGetValue(towar, out var c))
            return 0;

        float mult = GetSellMultiplier(miasto, towar);
        return Mathf.RoundToInt(c.price * mult);
    }
}
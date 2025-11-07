using System;
using UnityEngine;

public class CzasGry : MonoBehaviour
{
    public static CzasGry Instance { get; private set; }

    [Header("Tempo czasu")]
    [Tooltip("Ile minut gry up³ywa w 1 sekundê rzeczywist¹")]
    public float minutyNaSekunde = 1f;

    [Header("Aktualny czas gry")]
    public int dzien = 1;
    public int godzina = 8;
    public int minuta = 0;

    public event Action OnZmianaCzasu;

    private float akumulator;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Update()
    {
        akumulator += Time.deltaTime * minutyNaSekunde;
        while (akumulator >= 1f)
        {
            akumulator -= 1f;
            minuta++;
            if (minuta >= 60) { minuta = 0; godzina++; }
            if (godzina >= 24) { godzina = 0; dzien++; }
            OnZmianaCzasu?.Invoke();
        }
    }

    public string FormatujGodzine() => $"{godzina:00}:{minuta:00}";
    public string FormatujDzienIGodzine() => $"Day {dzien}  {godzina:00}:{minuta:00}";
}

using System;
using UnityEngine;

public class CzasGry : MonoBehaviour
{
    public static CzasGry Instance { get; private set; }

    [Header("Tempo czasu")]
    [Tooltip("Ile sekund rzeczywistych musi min¹æ, by up³yn¹³ 1 dzieñ w grze")]
    public float realneSekundyNaDzien = 4f;

    [Header("Sterowanie")]
    public bool czasPlynie = false;

    [Header("Kalendarz")]
    public int dzien = 1;
    public int miesiac = 1;
    public int rok = 2305;

    public event Action OnZmianaCzasu;
    private float _akumulator;
    private readonly int[] _dniWMiesiacach = { 0, 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Update()
    {
        if (!czasPlynie) return;

        _akumulator += Time.deltaTime;

        if (_akumulator >= realneSekundyNaDzien)
        {
            _akumulator -= realneSekundyNaDzien;

            PrzejdzDoNastepnegoDnia();
        }
    }

    private void PrzejdzDoNastepnegoDnia()
    {
        dzien++;

        int maxDni = _dniWMiesiacach[miesiac];

        if (miesiac == 2 && CzyRokPrzestepny(rok)) maxDni = 29;

        if (dzien > maxDni)
        {
            dzien = 1;
            miesiac++;

            if (miesiac > 12)
            {
                miesiac = 1;
                rok++;
            }
        }

        OnZmianaCzasu?.Invoke();
    }

    private bool CzyRokPrzestepny(int y)
    {
        return (y % 4 == 0 && (y % 100 != 0 || y % 400 == 0));
    }

    public string FormatujDate()
    {
        return $"{dzien:D2}.{miesiac:D2}.{rok}";
    }

    public string FormatujTekstowo()
    {
        return $"Dzieñ {dzien}, {rok}";
    }
}
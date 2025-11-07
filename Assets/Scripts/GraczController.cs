using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraczController : MonoBehaviour
{
    [Header("Ruch")]
    [Range(0.5f, 50f)] public float predkosc = 6f; 
    public bool czyWTrasie { get; private set; }
    public Miasto AktualneMiasto { get; private set; }

    public class InfoPodrozy
    {
        public string nazwaStart;
        public string nazwaCel;
        public float dystansCalkowity;   
        public float sekundyRzeczywiste; 
        public float minutyGryCalkowite; 
    }
    public event Action<InfoPodrozy> OnPodrozStart;
    public event Action<float> OnPodrozPostep; 
    public event Action OnPodrozKoniec;

    public void UstawStart(Miasto miasto)
    {
        AktualneMiasto = miasto;
        transform.position = miasto.transform.position;
    }

    public void RuszajPoSciezce(List<Miasto> sciezka)
    {
        if (sciezka == null || sciezka.Count < 2 || czyWTrasie) return;
        StopAllCoroutines();
        StartCoroutine(RuchPoPelnejSciezce(sciezka));
    }

    private IEnumerator RuchPoPelnejSciezce(List<Miasto> sciezka)
    {
        czyWTrasie = true;

        List<Vector3> punktyPelne = ZbudujPelnaListePunktow(sciezka);
        if (punktyPelne == null || punktyPelne.Count < 2)
        {
            Debug.LogWarning("Brak punktów trasy.");
            czyWTrasie = false;
            yield break;
        }

        float dystansCalk = PoliczDystans(punktyPelne);
        float sekReal = dystansCalk / Mathf.Max(0.0001f, predkosc);
        float minGry = sekReal * ((CzasGry.Instance != null) ? CzasGry.Instance.minutyNaSekunde : 1f);

        OnPodrozStart?.Invoke(new InfoPodrozy
        {
            nazwaStart = sciezka[0].nazwa,
            nazwaCel = sciezka[sciezka.Count - 1].nazwa,
            dystansCalkowity = dystansCalk,
            sekundyRzeczywiste = sekReal,
            minutyGryCalkowite = minGry
        });

        float pokonana = 0f;
        for (int i = 1; i < punktyPelne.Count; i++)
        {
            Vector3 cel = punktyPelne[i];

            while (Vector3.Distance(transform.position, cel) > 0.01f)
            {
                float dystPrzed = Vector3.Distance(transform.position, cel);
                float krok = predkosc * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, cel, krok);

                float realnie = Mathf.Min(krok, dystPrzed);
                pokonana += realnie;

                OnPodrozPostep?.Invoke(Mathf.Clamp01(pokonana / Mathf.Max(0.0001f, dystansCalk)));
                yield return null;
            }

            transform.position = cel;
        }

        AktualneMiasto = sciezka[sciezka.Count - 1];
        czyWTrasie = false;
        OnPodrozPostep?.Invoke(1f);
        OnPodrozKoniec?.Invoke();
        MapaGry.Instance?.PoDotarciuDoCelu(AktualneMiasto);
    }

    private List<Vector3> ZbudujPelnaListePunktow(List<Miasto> sciezka)
    {
        List<Vector3> wynik = new List<Vector3>();
        if (sciezka == null || sciezka.Count < 2) return wynik;

        wynik.Add(transform.position);

        for (int i = 0; i < sciezka.Count - 1; i++)
        {
            Miasto a = sciezka[i];
            Miasto b = sciezka[i + 1];
            Droga d = ZnajdzDroge(a, b);
            if (d == null)
            {
                Debug.LogError($"Brak drogi z {a.nazwa} do {b.nazwa}");
                continue;
            }

            var punkty = d.PobierzSciezke();
            if (punkty == null || punkty.Count < 2) continue;

            float distStart = Vector3.Distance(punkty[0], a.transform.position);
            float distEnd = Vector3.Distance(punkty[punkty.Count - 1], a.transform.position);
            if (distEnd < distStart) punkty.Reverse();

            for (int k = 0; k < punkty.Count; k++)
            {
                if (k == 0 && wynik.Count > 0 && Vector3.Distance(wynik[wynik.Count - 1], punkty[k]) < 0.001f)
                    continue;
                wynik.Add(punkty[k]);
            }
        }

        return wynik;
    }

    private float PoliczDystans(List<Vector3> punkty)
    {
        float suma = 0f;
        for (int i = 1; i < punkty.Count; i++)
            suma += Vector3.Distance(punkty[i - 1], punkty[i]);
        return suma;
    }

    private Droga ZnajdzDroge(Miasto a, Miasto b)
    {
        foreach (var d in a.drogi)
        {
            if ((d.miastoA == a && d.miastoB == b) || (d.miastoA == b && d.miastoB == a))
                return d;
        }
        return null;
    }
}

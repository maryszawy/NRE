using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Droga : MonoBehaviour
{
    public Miasto miastoA;
    public Miasto miastoB;

    [Header("Render")]
    [Range(0.05f, 1f)] public float szerokosc = 0.25f;
    [Range(0, 8)] public int cornerVerts = 4;
    [Range(0, 8)] public int capVerts = 4;
    public bool zszywajKoncowki = true;

    private LineRenderer lr;

    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
        UstawRenderer();
        ZszyjKoncowkiIZ();
    }

    private void LateUpdate()
    {
        ZszyjKoncowkiIZ();
    }

    private void OnValidate()
    {
        if (!lr) lr = GetComponent<LineRenderer>();
        UstawRenderer();
        ZszyjKoncowkiIZ();
    }

    private void UstawRenderer()
    {
        if (!lr) return;
        lr.useWorldSpace = true;
        lr.alignment = LineAlignment.View;
        lr.startWidth = szerokosc;
        lr.endWidth = szerokosc;
        lr.numCornerVertices = cornerVerts;
        lr.numCapVertices = capVerts;

        var col = lr.colorGradient;
        
        var c0 = col.colorKeys.Length > 0 ? col.colorKeys[0].color : Color.white;
        Gradient g = new Gradient();
        g.SetKeys(
            new[] { new GradientColorKey(c0, 0f), new GradientColorKey(c0, 1f) },
            new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) }
        );
        lr.colorGradient = g;
    }

    private void ZszyjKoncowkiIZ()
    {
        if (!lr || lr.positionCount < 2) return;

        
        var n = lr.positionCount;
        var buf = new Vector3[n];
        lr.GetPositions(buf);

        for (int i = 0; i < n; i++)
            buf[i].z = 0f;

        if (zszywajKoncowki)
        {
            if (miastoA) buf[0] = new Vector3(miastoA.transform.position.x, miastoA.transform.position.y, 0f);
            if (miastoB) buf[n - 1] = new Vector3(miastoB.transform.position.x, miastoB.transform.position.y, 0f);
        }

        lr.SetPositions(buf);
    }

    public List<Vector3> PobierzSciezke()
    {
        if (!lr || lr.positionCount < 2) return new List<Vector3>();
        var arr = new Vector3[lr.positionCount];
        lr.GetPositions(arr);
        
        for (int i = 0; i < arr.Length; i++) arr[i].z = 0f;
        if (miastoA) arr[0] = new Vector3(miastoA.transform.position.x, miastoA.transform.position.y, 0f);
        if (miastoB) arr[arr.Length - 1] = new Vector3(miastoB.transform.position.x, miastoB.transform.position.y, 0f);
        return new List<Vector3>(arr);
    }

    public float Dlugosc()
    {
        var s = PobierzSciezke();
        float suma = 0f;
        for (int i = 1; i < s.Count; i++)
            suma += Vector3.Distance(s[i - 1], s[i]);
        return suma;
    }

    public void UstawWidocznosc(bool v) { if (lr) lr.enabled = v; }
}

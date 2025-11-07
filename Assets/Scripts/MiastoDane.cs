using System;
using System.Collections.Generic;

[Serializable]
public class MiastoDane
{
    public string nazwa;
    public string typ;
    public List<Towar> towary = new List<Towar>();
}

using System.IO;
using UnityEngine;

public static class SciezkiZapisu
{
    public static string Folder =>
        Path.Combine(Application.dataPath, "Data/Save");

    public static string PlikGracza =>
        Path.Combine(Folder, "player.json");

    public static string PlikMiast =>
        Path.Combine(Folder, "miasta.json");
}

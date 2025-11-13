using UnityEngine;
using System.IO;

public static class SaveSystem
{
    static string Path => System.IO.Path.Combine(Application.persistentDataPath, "save.json");

    public static void Save(SaveData data)
    {
        var json = JsonUtility.ToJson(data);
        File.WriteAllText(Path, json);
    }

    public static bool Exists() => File.Exists(Path);

    public static SaveData Load()
    {
        if (!Exists()) return null;
        var json = File.ReadAllText(Path);
        return JsonUtility.FromJson<SaveData>(json);
    }

    public static void Delete()
    {
        if (Exists()) File.Delete(Path);
    }
}

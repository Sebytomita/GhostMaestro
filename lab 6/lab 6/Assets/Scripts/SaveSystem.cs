using UnityEngine;

[System.Serializable]
public class SaveData { public int checkpointIndex = 0; }

public static class SaveSystem
{
    const string Key = "SAVE_DATA_V1";

    public static bool HasSave() => PlayerPrefs.HasKey(Key);

    public static void Save(SaveData data)
    {
        var json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(Key, json);
        PlayerPrefs.Save();
    }

    public static SaveData Load()
    {
        if (!HasSave()) return null;
        return JsonUtility.FromJson<SaveData>(PlayerPrefs.GetString(Key));
    }

    public static void Clear() => PlayerPrefs.DeleteKey(Key);
}

using System;
using System.IO;
using UnityEngine;

public class SaveService
{
    public const int CurrentVersion = 1;
    private const string SaveFileName = "save_0.json";

    public SaveData PendingLoad { get; set; }

    private string SaveFilePath => Path.Combine(Application.persistentDataPath, SaveFileName);

    public bool HasSave() => File.Exists(SaveFilePath);

    public bool Save(SaveData data)
    {
        try
        {
            if (data == null) return false;
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(SaveFilePath, json);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveService] Error saving game: {e}");
            return false;
        }
    }

    public SaveData Load()
    {
        try
        {
            if (!HasSave()) return null;
            string json = File.ReadAllText(SaveFilePath);
            return JsonUtility.FromJson<SaveData>(json);
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveService] Error loading game: {e}");
            return null;
        }
    }

    public void DeleteSave()
    {
        try
        {
            if (HasSave())
            {
                File.Delete(SaveFilePath);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveService] Error deleting save: {e}");
        }
    }
}

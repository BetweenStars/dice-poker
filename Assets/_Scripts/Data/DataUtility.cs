using System;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public static class DataUtility
{
    public static void Save(int slotIndex, GameData data)
    {
        try
        {
            string path = GetPath(slotIndex);

            string json = JsonConvert.SerializeObject(data, Formatting.Indented);

            File.WriteAllText(path, json);
            //Debug.Log($"Save Data Successfully. Saved at {path}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Save Failed\n{e}");
        }
    }
    public static GameData Load(int slotIndex)
    {
        try
        {
            string path = GetPath(slotIndex);

            if (!File.Exists(path))
                return new GameData();

            string json = File.ReadAllText(path);

            //Debug.Log("Load Data Successfully");

            return JsonConvert.DeserializeObject<GameData>(json) ?? new GameData();
        }
        catch (Exception e)
        {
            Debug.LogError($"Load Failed\n{e}");
            return new GameData();
        }
    }
    public static void Delete(int slotIndex)
    {
        try
        {
            string path = GetPath(slotIndex);

            if (!File.Exists(path))
                return;

            File.Delete(path);
            //Debug.Log("Delete Data Successfully");
        }
        catch (Exception e)
        {
            Debug.LogError($"Delete Failed\n{e}");
        }
    }
    public static bool HasData(int slotIndex) => File.Exists(GetPath(slotIndex));
    public static string GetPath(int index) => Path.Combine(Application.persistentDataPath, $"save{index}.json");
}

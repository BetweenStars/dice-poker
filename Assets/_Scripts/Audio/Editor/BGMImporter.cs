using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class BGMImporter : AssetPostprocessor
{
    private const string BgmFolder = "Assets/Audio/Clips/BGM/";
    private const string BgmDataFolder = "Assets/_Data/BGM Datas";
    private const string AddressPrefix = "BGM/";
    private const string BgmLabel = "BGM";
    private const string HomeLabel = "Home";

    private static readonly HashSet<string> PendingAssetPaths = new();
    private static bool isProcessingQueued;

    private static void OnPostprocessAllAssets(
        string[] importedAssets,
        string[] deletedAssets,
        string[] movedAssets,
        string[] movedFromAssetPaths)
    {
        foreach (string assetPath in importedAssets)
        {
            QueueBGMImport(assetPath);
        }

        foreach (string assetPath in movedAssets)
        {
            QueueBGMImport(assetPath);
        }
    }

    private static void QueueBGMImport(string assetPath)
    {
        if (!IsBGMClip(assetPath)) { return; }

        PendingAssetPaths.Add(assetPath);

        if (isProcessingQueued) { return; }

        isProcessingQueued = true;
        EditorApplication.delayCall += ProcessQueuedBGMImports;
    }

    private static void ProcessQueuedBGMImports()
    {
        isProcessingQueued = false;

        foreach (string assetPath in PendingAssetPaths)
        {
            ImportBGM(assetPath);
        }

        PendingAssetPaths.Clear();
        AssetDatabase.SaveAssets();
    }

    private static void ImportBGM(string assetPath)
    {
        if (!IsBGMClip(assetPath)) { return; }

        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings == null) { return; }

        string guid = AssetDatabase.AssetPathToGUID(assetPath);
        if (string.IsNullOrEmpty(guid)) { return; }

        AddressableAssetGroup group = settings.DefaultGroup;
        if (group == null) { return; }

        AddressableAssetEntry entry = settings.CreateOrMoveEntry(guid, group);
        entry.address = AddressPrefix + Path.GetFileNameWithoutExtension(assetPath);

        CreateOrUpdateBGMDefinition(assetPath, guid, settings, group);

        settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryModified, entry, true);
    }

    private static void CreateOrUpdateBGMDefinition(
        string clipPath,
        string clipGuid,
        AddressableAssetSettings settings,
        AddressableAssetGroup group)
    {
        if (!AssetDatabase.IsValidFolder(BgmDataFolder))
        {
            Directory.CreateDirectory(BgmDataFolder);
            AssetDatabase.Refresh();
        }

        string fileName = Path.GetFileNameWithoutExtension(clipPath);
        string assetPath = $"{BgmDataFolder}/{fileName}.asset";

        BGMDefinition definition = AssetDatabase.LoadAssetAtPath<BGMDefinition>(assetPath);

        if (definition == null)
        {
            definition = ScriptableObject.CreateInstance<BGMDefinition>();
            AssetDatabase.CreateAsset(definition, assetPath);
        }

        AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(clipPath);

        definition.BGMId = fileName;
        definition.DisplayText = fileName;
        definition.LengthSecond = clip == null ? 0 : Mathf.RoundToInt(clip.length);
        definition.clipRef = new AssetReferenceT<AudioClip>(clipGuid);

        EditorUtility.SetDirty(definition);

        string definitionGuid = AssetDatabase.AssetPathToGUID(assetPath);
        if (string.IsNullOrEmpty(definitionGuid)) { return; }

        AddressableAssetEntry definitionEntry = settings.CreateOrMoveEntry(definitionGuid, group);
        definitionEntry.SetLabel(BgmLabel, true, true);
        definitionEntry.SetLabel(HomeLabel, true, true);

        settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryModified, definitionEntry, true);
    }

    private static bool IsBGMClip(string assetPath)
    {
        return assetPath.StartsWith(BgmFolder)
            && string.Equals(Path.GetExtension(assetPath), ".mp3", System.StringComparison.OrdinalIgnoreCase);
    }
}

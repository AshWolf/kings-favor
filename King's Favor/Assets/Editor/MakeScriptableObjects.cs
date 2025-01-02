using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class MakeScriptableObject
{
    [MenuItem("Assets/Create/Card")]
    public static void CreateCardAsset()
    {
        CreateAsset<Card>("Card");
    }

    [MenuItem("Assets/Create/MapData")]
    public static void CreateMapDataAsset()
    {
        CreateAsset<MapData>("MapData");
    }

    private static void CreateAsset<T>(string name) where T : ScriptableObject
    {
        T asset = ScriptableObject.CreateInstance<T>();

        AssetDatabase.CreateAsset(asset, Path.Join(GetCurrentPath(), $"{name}.asset"));
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
    }

    // https://discussions.unity.com/t/how-to-get-currently-selected-folder-for-putting-new-asset-into/439128/3
    private static string GetCurrentPath()
    {
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (path == "")
        {
            path = "Assets";
        }
        else if (Path.GetExtension(path) != "")
        {
            path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
        }
        return path;
    }
}

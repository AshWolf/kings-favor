using UnityEditor;
using UnityEngine;
using System.IO;

public class CSVImporter : EditorWindow
{
    private string csvFilePath = "Assets/Cards/Cards.csv";

    [MenuItem("Tools/CSV Importer")]
    public static void ShowWindow()
    {
        GetWindow<CSVImporter>("CSV Importer");
    }

    void OnGUI()
    {
        GUILayout.Label("Import CSV and create Cards", EditorStyles.boldLabel);

        csvFilePath = EditorGUILayout.TextField("CSV File Path", csvFilePath);

        if (GUILayout.Button("Import CSV"))
        {
            ImportCSV();
        }
    }

    private void ImportCSV()
    {
        if (!File.Exists(csvFilePath))
        {
            Debug.LogError("CSV file not found: " + csvFilePath);
            return;
        }

        string[] lines = File.ReadAllLines(csvFilePath);
        foreach (string line in lines)
        {
            string[] values = line.Split(',');

            if (values.Length < 6)
            {
                Debug.LogWarning("Skipping invalid line: " + line);
                continue;
            }

            Card card = ScriptableObject.CreateInstance<Card>();
            card.title = values[0];

            card.tier = int.Parse(values[2]);
            Debug.Log(values[5]);
            card.cost = int.Parse(values[3]);
            card.copies = int.Parse(values[5]);
            
            card.description = values[4];

            string assetPath = "Assets/Cards/" + card.title + ".asset";
            AssetDatabase.CreateAsset(card, assetPath);
        }

        AssetDatabase.SaveAssets();
        Debug.Log("CSV imported successfully!");
    }
}

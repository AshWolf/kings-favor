using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Map : MonoBehaviour
{
    public GameObject HexPrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

[CustomEditor(typeof(Map))]
public class MapEditor : Editor
{
    private void GenerateMap(Map map)
    {
        for(int radius = 0; radius <= 5; radius++)
        {
            foreach (Hex hex in Hex.Ring(radius))
            {
                var pixel = hex.ToPixel(size: 0.5f);
                var obj = GameObject.Instantiate(map.HexPrefab, map.transform);
                obj.name = hex.ToString();
                obj.transform.localPosition = new(pixel.x, 0, pixel.y);
            }
        }
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Map map = (Map)target;

        if (GUILayout.Button("Generate Map"))
        {
            GenerateMap(map);
        }
    }
}

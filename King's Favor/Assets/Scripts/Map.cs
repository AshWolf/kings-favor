using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


[System.Serializable]
public struct Wall {
    public Hex First;
    public Hex Second;
}

public class Map : MonoBehaviour
{
    public GameObject HexPrefab;
    public GameObject WallPrefab;

    public List<Wall> Walls = new();

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
        float hexSize = 0.51f;

        for (int radius = 0; radius <= 5; radius++)
        {
            foreach (Hex hex in Hex.Ring(radius))
            {
                var pixel = hex.ToPixel(hexSize);
                var obj = GameObject.Instantiate(map.HexPrefab, map.transform);
                obj.name = hex.ToString();
                obj.transform.localPosition = new(pixel.x, 0, pixel.y);
                obj.GetComponent<Tile>().Hex = hex;
            }
        }

        foreach (Wall wall in map.Walls)
        {
            var p1 = wall.First.ToPixel(hexSize);
            var p2 = wall.Second.ToPixel(hexSize);
            var center = (p1 + p2) / 2;
            var perp = p2 - p1;

            var obj = GameObject.Instantiate(map.WallPrefab, map.transform);
            // obj.name = wall.ToString();
            obj.transform.localPosition = new(center.x, 0, center.y);
            obj.transform.right = new(perp.x, 0, perp.y);
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

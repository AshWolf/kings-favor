using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public struct Wall
{
    public Hex First;
    public Hex Second;
    public override int GetHashCode() => (First, Second).GetHashCode();
}

[System.Serializable]
public struct Shop {
    public Hex Hex;
    public int Tier;
    public ShopType Type;

    public enum ShopType { 
        Action, Magic, Ally, Item
    }
}

public class MapData : ScriptableObject
{
    public int Radius;
    public List<Wall> Walls = new();
    public List<Hex> Portals;
    public List<Shop> Shops;
}


[CustomEditor(typeof(MapData))]
public class MapDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MapData map = (MapData)target;

        if (GUILayout.Button("Reflect Walls"))
        {
            HashSet<Wall> walls = new();
            walls.AddRange(map.Walls);
            walls.AddRange(map.Walls.Select(w => new Wall() { First = -w.First, Second = -w.Second } ));
            walls.AddRange(map.Walls.Select(w => new Wall() { First = w.First.ReflectQ(), Second = w.Second.ReflectQ() }));
            walls.AddRange(map.Walls.Select(w => new Wall() { First = -w.First.ReflectQ(), Second = -w.Second.ReflectQ() }));
            map.Walls = walls.ToList();
        }
    }
}

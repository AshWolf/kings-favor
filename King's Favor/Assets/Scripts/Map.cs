using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Timeline;


[System.Serializable]
public struct Wall {
    public Hex First;
    public Hex Second;
}

public class Map : MonoBehaviour
{
    public GameObject HexPrefab;
    public GameObject WallPrefab;

    public Transform Marker;

    public List<Wall> Walls = new();

    public float HexSize = 0.51f;
    public int Radius = 5;

    private Dictionary<Hex, Tile> Tiles = new();

    // Start is called before the first frame update
    void Start()
    {
        foreach(var tile in GetComponentsInChildren<Tile>())
        {
            Tiles[tile.Hex] = tile;
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach(var tile in Tiles.Values)
        {
            tile.GetComponent<MeshRenderer>().material.color = Color.green;
        }

        var plane = new Plane(transform.up, transform.position);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (plane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);

            hitPoint = transform.InverseTransformPoint(hitPoint);

            Hex hex = Hex.PixelToHex(new(hitPoint.x, hitPoint.z), size: HexSize);

            if (IsHexValid(hex))
            {
                // TODO: currently is just a demo to highlight hexes within a range of 1
                foreach (Hex hexInRange in GetHexesInRange(hex, 1))
                {
                    var obj = Tiles[hexInRange];
                    obj.GetComponent<MeshRenderer>().material.color = Color.red;
                }
            }
        }
    }

    private HashSet<Hex> GetHexesInRange(Hex center, int range)
    {
        HashSet<Hex> visited = new();
        List<Hex> queue = new();

        visited.Add(center);
        queue.Add(center);

        for (int i = 0; i < range; i++)
        {
            List<Hex> nextQueue = new();
            foreach (Hex hex in queue)
            {
                foreach (Hex direction in Hex.Directions)
                {
                    Hex candidate = hex + direction;
                    if (IsHexValid(candidate) && !visited.Contains(candidate) && !HasWall(hex, candidate))
                    {
                        visited.Add(candidate);
                        nextQueue.Add(candidate);
                    }
                }
            }
            queue = nextQueue;
        }

        return visited;
    }

    private bool HasWall(Hex a, Hex b)
    {
        return Walls.Any(wall => (wall.First == a && wall.Second == b) || (wall.First == b && wall.Second == a));
    }

    private bool IsHexValid(Hex hex)
    {
        return hex.Magnitude() <= Radius;
    }
}

[CustomEditor(typeof(Map))]
public class MapEditor : Editor
{
    private void GenerateMap(Map map)
    {

        for (int radius = 0; radius <= map.Radius; radius++)
        {
            foreach (Hex hex in Hex.Ring(radius))
            {
                var pixel = hex.ToPixel(map.HexSize);
                var obj = GameObject.Instantiate(map.HexPrefab, map.transform);
                obj.name = hex.ToString();
                obj.transform.localPosition = new(pixel.x, 0, pixel.y);
                obj.GetComponent<Tile>().Hex = hex;
            }
        }

        foreach (Wall wall in map.Walls)
        {
            var p1 = wall.First.ToPixel(map.HexSize);
            var p2 = wall.Second.ToPixel(map.HexSize);
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

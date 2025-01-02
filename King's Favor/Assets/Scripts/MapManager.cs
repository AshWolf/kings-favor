using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Timeline;


public class MapManager : MonoBehaviour
{

    public MapData MapData;

    public float HexSize = 0.51f;

    [Space]

    public GameObject HexPrefab;
    public GameObject WallPrefab;
    public GameObject PortalPrefab;

    private Dictionary<Hex, IsTile> Tiles = new();

    // Start is called before the first frame update
    void Start()
    {
        foreach(var tile in GetComponentsInChildren<IsTile>())
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
        return MapData.Walls.Any(wall => (wall.First == a && wall.Second == b) || (wall.First == b && wall.Second == a));
    }

    private bool IsHexValid(Hex hex)
    {
        return hex.Magnitude() <= MapData.Radius;
    }
}

[CustomEditor(typeof(MapManager))]
public class MapManagerEditor : Editor
{
    private static void DestoryAllChildrenImmediate<T>(MonoBehaviour parent) where T : MonoBehaviour {
        foreach (var obj in parent.GetComponentsInChildren<T>())
        {
            DestroyImmediate(obj.gameObject);
        }
    }

    private void GenerateMap(MapManager map)
    {
        DestoryAllChildrenImmediate<IsTile>(map);
        DestoryAllChildrenImmediate<IsWall>(map);
        DestoryAllChildrenImmediate<IsPortal>(map);

        for (int radius = 0; radius <= map.MapData.Radius; radius++)
        {
            foreach (Hex hex in Hex.Ring(radius))
            {
                var pixel = hex.ToPixel(map.HexSize);
                var obj = GameObject.Instantiate(map.HexPrefab, map.transform);
                obj.name = hex.ToString();
                obj.transform.localPosition = new(pixel.x, 0, pixel.y);
                obj.GetComponent<IsTile>().Hex = hex;
            }
        }

        foreach (Wall wall in map.MapData.Walls)
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

        for(int i = 0; i < map.MapData.Portals.Count; ++i)
        {
            var hex = map.MapData.Portals[i];
            var pixel = hex.ToPixel(map.HexSize);
            var obj = GameObject.Instantiate(map.PortalPrefab, map.transform);
            obj.name = $"Portal {i+1}";
            obj.transform.localPosition = new(pixel.x, 0.1f, pixel.y);
        }
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MapManager map = (MapManager)target;

        if (GUILayout.Button("Generate Map"))
        {
            GenerateMap(map);
        }
    }
}

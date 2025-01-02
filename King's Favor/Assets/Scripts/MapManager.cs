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

    public Transform Meeple;

    public LineRenderer PathRenderer;

    [Space]

    public GameObject HexPrefab;
    public GameObject WallPrefab;
    public GameObject PortalPrefab;

    private Dictionary<Hex, IsTile> _tiles = new();

    // Map state
    private bool _meepleSelected;
    private int _meepleRange = 2;
    private Hex _meeplePose = Hex.Zero;

    // Start is called before the first frame update
    void Start()
    {
        foreach(var tile in GetComponentsInChildren<IsTile>())
        {
            _tiles[tile.Hex] = tile;
        }
    }

    // Update is called once per frame
    void Update()
    {
        var plane = new Plane(transform.up, transform.position);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (plane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);

            hitPoint = transform.InverseTransformPoint(hitPoint);

            Hex hex = Hex.PixelToHex(new(hitPoint.x, hitPoint.z), size: HexSize);

            HandleHitHex(IsHexValid(hex), hex);
        } else
        {
            HandleHitHex(false, Hex.Zero);
        }
    }

    private void HandleHitHex(bool isValid, Hex hex)
    {
        foreach (var tile in _tiles.Values)
        {
            tile.SetColor(Color.green);
        }

        if(!isValid)
        {
            return;
        }

        var meepleRangeInfo = ComputeRangeInfo(_meeplePose, _meepleRange);
        bool clicked = Input.GetMouseButtonDown(0);

        if (_meepleSelected)
        {
            if (clicked)
            {
                if (hex == _meeplePose)
                {
                    _meepleSelected = false;
                }
                else if(meepleRangeInfo.ContainsKey(hex))
                {
                    // TODO: Move meeple
                    _meepleSelected = false;
                    _meeplePose = hex;
                    var pixel = hex.ToPixel(size: HexSize);
                    Vector3 newPosition = transform.TransformPoint(new(pixel.x, 0, pixel.y));
                    Meeple.position = new(newPosition.x, Meeple.position.y, newPosition.z);
                    Debug.Log($"Spent {meepleRangeInfo[hex].Range} movement");
                }
                else
                {
                    _meepleSelected = false;
                }
            }
        }
        else
        {
            if (hex == _meeplePose)
            {
                if (clicked)
                {
                    _meepleSelected = true;
                }
            }
        }

        if(hex == _meeplePose || _meepleSelected)
        {
            foreach (Hex hexInRange in meepleRangeInfo.Keys)
            {
                var tile = _tiles[hexInRange];
                tile.SetColor(Color.red);
            }
        }

        if(_meepleSelected && meepleRangeInfo.ContainsKey(hex))
        {
            _tiles[hex].SetColor(Color.yellow);

            PathRenderer.enabled = true;

            var path = GetPath(meepleRangeInfo, hex);
            PathRenderer.positionCount = path.Count;
            PathRenderer.SetPositions(
                path.Select(h => transform.TransformPoint(HexToPosition(h, 0.25f))).ToArray()
            );
        }
        else
        {
            PathRenderer.enabled = false;
        }
    }


    private struct RangeHexInfo {
        public int Range;
        public Hex Direction;
    }

    private Dictionary<Hex, RangeHexInfo> ComputeRangeInfo(Hex center, int range)
    {
        Dictionary<Hex, RangeHexInfo> result = new();
        List<Hex> queue = new();

        result.Add(center, new () { Range = 0, Direction = Hex.Zero });
        queue.Add(center);

        for (int i = 0; i < range; i++)
        {
            List<Hex> nextQueue = new();
            foreach (Hex hex in queue)
            {
                foreach (Hex direction in Hex.Directions)
                {
                    Hex candidate = hex + direction;
                    if (IsHexValid(candidate) && !result.ContainsKey(candidate) && !HasWall(hex, candidate))
                    {
                        result.Add(candidate, new() { Range = i + 1, Direction = direction });
                        nextQueue.Add(candidate);
                    }
                }
            }
            queue = nextQueue;
        }

        return result;
    }

    private List<Hex> GetPath(Dictionary<Hex, RangeHexInfo> rangeInfoMap, Hex endpoint)
    {
        List<Hex> path = new() { endpoint };

        Hex hex = endpoint;
        while (rangeInfoMap[hex].Range > 0)
        {
            hex -= rangeInfoMap[hex].Direction;
            path.Add(hex);
        }

        path.Reverse();
        return path;
    }

    private Vector3 HexToPosition(Hex hex, float y=0f)
    {
        var pixel = hex.ToPixel(HexSize);
        return new(pixel.x, y, pixel.y);
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

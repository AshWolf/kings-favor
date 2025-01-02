using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Wall
{
    public Hex First;
    public Hex Second;
}

public class MapData : ScriptableObject
{
    public int Radius;
    public List<Wall> Walls = new();
    public List<Hex> Portals;
}

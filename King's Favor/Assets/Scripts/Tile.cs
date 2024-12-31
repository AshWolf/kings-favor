using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Hex Hex;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDrawGizmos()
    {
        Handles.Label(transform.position, Hex.ToString(), new GUIStyle { alignment = TextAnchor.MiddleCenter });
    }
}

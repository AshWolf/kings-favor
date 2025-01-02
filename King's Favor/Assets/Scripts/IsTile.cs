using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class IsTile : MonoBehaviour
{
    public Hex Hex;

    private MeshRenderer meshRenderer;

    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetColor(Color color)
    {
        meshRenderer.material.color = color;
    }

    void OnDrawGizmos()
    {
        Handles.Label(transform.position, Hex.ToString(), new GUIStyle { alignment = TextAnchor.MiddleCenter });
    }
}

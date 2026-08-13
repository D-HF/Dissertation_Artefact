using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[ExecuteInEditMode()]
public class TileInfo : MonoBehaviour
{
    //attributes
    public string tileName;
    public string tileType;
    public Material tileMaterial;
    [SerializeField] GameObject tile;
    public TileInfo[] northNeighbours;
    public TileInfo[] eastNeighbours;
    public TileInfo[] southNeighbours;
    public TileInfo[] westNeighbours;

    private void Awake()
    { 
        Renderer renderer = GetComponent<Renderer>();
        tileMaterial = renderer.GetComponent<Renderer>().sharedMaterial;
        tileName = tile.name;
    }
}

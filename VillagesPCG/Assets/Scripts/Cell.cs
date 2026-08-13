using UnityEngine;

public class Cell : MonoBehaviour
{
    public bool collapsed;
    public TileInfo[] tileOptions;

    public void CreateCell(bool collapseState, TileInfo[] tiles)
    {
        collapsed = collapseState;
        tileOptions = tiles;
    }

    public void RecreateCell(TileInfo[] tiles)
    {
        tileOptions = tiles; 
    }
}

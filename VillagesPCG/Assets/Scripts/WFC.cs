using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class WFC : MonoBehaviour
{
    public int dimensions;
    public TileInfo[] tileObj;
    public List<Cell> gridComponents;
    public Cell cellObj;
    public TileInfo backup;
    private int iteration;
    public float cellSpace;
    public float speed;
    private void Awake()
    {
        gridComponents = new List<Cell>();
        InitializeGrid();
    }
    
    void InitializeGrid()
    {
        for(int y = 0; y < dimensions; y++)
        {
            for(int x = 0; x < dimensions; x++)
            {
                Cell newCell = Instantiate(cellObj, new Vector3(x * cellSpace, 0, y * cellSpace), Quaternion.identity); //make a new blank cell at position x,y, with no rotation
                newCell.CreateCell(false, tileObj); //cell is not collapsed, has a list of Tiles
                gridComponents.Add(newCell); //add created cell to a list of all grid cells
            }
        }
        StartCoroutine(CheckEntropy());
    }
    IEnumerator CheckEntropy()
    {
        List<Cell> tempGrid = new List<Cell>(gridComponents);
        tempGrid.RemoveAll(c => c.collapsed);
        tempGrid.Sort((a, b) => a.tileOptions.Length - b.tileOptions.Length);
        tempGrid.RemoveAll(a => a.tileOptions.Length != tempGrid[0].tileOptions.Length);

        yield return new WaitForSeconds(speed);
        CollapseCell(tempGrid);
    }
    void CollapseCell(List<Cell> tempGrid)
    {
        int randIndex = UnityEngine.Random.Range(0, tempGrid.Count); //random tile in grid
        Cell cellToCollapse = tempGrid[randIndex];

        cellToCollapse.collapsed = true;
        try
        {
            TileInfo selectedTiles = cellToCollapse.tileOptions[UnityEngine.Random.Range(0, cellToCollapse.tileOptions.Length)];
            cellToCollapse.tileOptions = new TileInfo[] { selectedTiles };
        }
        catch
        {
            TileInfo selectedTiles = backup;
            cellToCollapse.tileOptions = new TileInfo[] { selectedTiles };
        }

        TileInfo foundTile = cellToCollapse.tileOptions[0];
        Instantiate(foundTile, cellToCollapse.transform.position, foundTile.transform.rotation); //instantiate that version of the tile

        Propogate();
    }
    void Propogate()
    {
        List<Cell> newGenCell = new List<Cell>(gridComponents); //copy of current grid

        for (int y = 0; y < dimensions; y++) //y coord
        {
            for (int x = 0; x < dimensions; x++) //x coord
            {
                var index = x + y * dimensions;
                
                if (gridComponents[index].collapsed)
                {
                    newGenCell[index] = gridComponents[index];
                }
                else
                {
                    List<TileInfo> options = new List<TileInfo>();
                    foreach (TileInfo t in tileObj)
                    {
                        options.Add(t);
                    }
                    if (y > 0)
                    {
                        Cell up = gridComponents[x + (y - 1) * dimensions];
                        List<TileInfo> validOptions = new List<TileInfo>();
                        foreach (TileInfo possibleOptions in up.tileOptions)
                        {
                            var validOption = Array.FindIndex(tileObj, obj => obj == possibleOptions);
                            var valid = tileObj[validOption].southNeighbours;

                            validOptions = validOptions.Concat(valid).ToList();
                        }
                        CheckValidity(options, validOptions);
                    }//north

                    if (x > 0)
                    {
                        Cell right = gridComponents[x - 1 + y * dimensions];
                        List<TileInfo> validOptions = new List<TileInfo>();

                        foreach (TileInfo possibleOptions in right.tileOptions)
                        {
                            var validOption = Array.FindIndex(tileObj, obj => obj == possibleOptions);
                            var valid = tileObj[validOption].westNeighbours;

                            validOptions = validOptions.Concat(valid).ToList();
                        }
                        CheckValidity(options, validOptions);
                    }//east

                    if (y < dimensions - 1)
                    {
                        Cell down = gridComponents[x + (y + 1) * dimensions];
                        List<TileInfo> validOptions = new List<TileInfo>();
                        foreach (TileInfo possibleOptions in down.tileOptions)
                        {
                            var validOption = Array.FindIndex(tileObj, obj => obj == possibleOptions);
                            var valid = tileObj[validOption].northNeighbours;
                            //Debug.Log("Index " + index + " array range " + validOption);

                            validOptions = validOptions.Concat(valid).ToList();
                        }
                        CheckValidity(options, validOptions);
                    }//south

                    if (x < dimensions - 1)
                    {
                        Cell left = gridComponents[x + 1 + y * dimensions];
                        List<TileInfo> validOptions = new List<TileInfo>();

                        foreach (TileInfo possibleOptions in left.tileOptions)
                        {
                            var validOption = Array.FindIndex(tileObj, obj => obj == possibleOptions);
                            var valid = tileObj[validOption].eastNeighbours;

                            validOptions = validOptions.Concat(valid).ToList();
                        }
                        CheckValidity(options, validOptions);
                    }//west

                    TileInfo[] newTileList = new TileInfo[options.Count];
                    for (int i = 0; i < options.Count; i++)
                    {
                        newTileList[i] = options[i];
                    }
                    newGenCell[index].RecreateCell(newTileList);
                }
            }
        }
        gridComponents = newGenCell;
        iteration++;
        if (iteration < dimensions * dimensions)
        {
            StartCoroutine(CheckEntropy());
        }
        else
        {
            Debug.Log("Complete Grid");
        }
    }

    void CheckValidity(List<TileInfo> optionList, List<TileInfo> validOption)
    {
        for (int x = optionList.Count - 1; x >= 0; x--)
        {
            var element = optionList[x];
            if (!validOption.Contains(element))
            {
                optionList.RemoveAt(x); //updated valid options list
            }
        }
    }
}

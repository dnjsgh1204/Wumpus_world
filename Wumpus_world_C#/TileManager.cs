using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public GameObject tilePrefab;
    public int rows = 4;
    public int columns = 4;
    private GameObject[,] tiles;

    void Start()
    {
        tiles = new GameObject[rows, columns];
        for (int x = 0; x < rows; x++)
        {
            for (int y = 0; y < columns; y++)
            {
                GameObject tile = Instantiate(tilePrefab, new Vector3(x, y, 0), Quaternion.identity);
                tile.transform.parent = this.transform;
                tile.name = "Tile_" + x + "_" + y;
                tile.GetComponent<SpriteRenderer>().sortingLayerName = "Background"; // Sorting Layer 설정
                tile.GetComponent<SpriteRenderer>().sortingOrder = 0; // Order in Layer 설정
                tiles[x, y] = tile;
            }
        }
    }
}

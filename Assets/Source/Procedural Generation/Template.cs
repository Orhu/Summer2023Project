using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Template : MonoBehaviour
{
    [Tooltip("")]
    [field: SerializeField] private Vector2Int _roomSize;
    public Vector2Int roomSize
    {
        get { return _roomSize; }
        set
        {
            _roomSize = value;
            tiles = new TemplateTile[_roomSize.x, _roomSize.y];
        }
    }

    [HideInInspector] public TemplateTile[,] tiles;

    public void SetTile(TemplateTile tile, Vector2Int location)
    {
        if (location.x < 0 || location.x >= roomSize.x || location.y < 0 || location.y >= roomSize.y)
        {
            throw new System.Exception("Cannot set location " + location.ToString() + " in template: Location out of range");
        }

        if (tile == null)
        {
            throw new System.Exception("Cannot set location " + location.ToString() + " in template: Tile is null");
        }

        tiles[location.x, location.y] = tile;
    }
}

/// <summary>
/// A pared-down version of a tile for use in the template. This information will be used to generate the tilemaps
/// </summary>
[System.Serializable]
public class TemplateTile
{
    [Tooltip("The type of this tile")]
    [SerializeField] public TileType tileType;

    [Tooltip("The preferred tile to spawn")]
    [SerializeField] public GameObject preferredTile;
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Trakcs the generic tiles and their types
/// </summary>
[CreateAssetMenu(fileName = "NewGenericTiles", menuName = "Tiles/GenericTiles", order = 1)]
public class GenericTiles : ScriptableObject
{
    [Tooltip("Stores a list of tile types to their generic versions")]
    public List<TileTypeToGenericTile> tileTypesToGenericTiles;

    /// <summary>
    /// Gets the generic tile associated with the given type
    /// </summary>
    /// <param name="tileType"> The tile type </param>
    /// <returns> The generic tile </returns>
    public Tile At(TileType tileType)
    {
        foreach (TileTypeToGenericTile tileTypeToGenericTile in tileTypesToGenericTiles)
        {
            if (tileTypeToGenericTile.tileType == tileType)
            {
                return tileTypeToGenericTile.genericTile;
            }
        }

        throw new System.Exception("No generic tile associated with type " + tileType.ToString());
    }
}

[System.Serializable]
public struct TileTypeToGenericTile
{
    [Tooltip("The type of the tile")]
    public TileType tileType;

    [Tooltip("The generic tile of this type")]
    public Tile genericTile;
}

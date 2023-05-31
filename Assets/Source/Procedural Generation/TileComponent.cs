using UnityEngine;

/// <summary>
/// A component that holds a tile, for easy access to prefabs without making the tile itself a monobehavior
/// </summary>
public class TileComponent : MonoBehaviour
{
    [Tooltip("The tile")]
    public Tile tile;
}

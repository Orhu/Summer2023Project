using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cardificer
{
    public class TemplateTile : MonoBehaviour
    {
        [Tooltip("The sprite to use to display this tile during template creation")]
        public Sprite sprite;

        [Tooltip("The type of this tile")]
        public TileType tileType = TileType.None;

        [Tooltip("The preferred tile to spawn")]
        public Tile preferredTile;
    }
}
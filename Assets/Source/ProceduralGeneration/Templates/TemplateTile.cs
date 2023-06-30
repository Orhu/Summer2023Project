using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// A tile for storing inside a template
    /// </summary>
    [System.Serializable]
    public class TemplateTile
    {
        [Tooltip("The sprite to use to display this tile during template creation")]
        public Sprite sprite;

        [Tooltip("The type of this tile")]
        public TileType tileType = TileType.None;

        [Tooltip("The preferred tile to spawn")]
        public Tile preferredTile;

        /// <summary>
        /// No default instantiate function for regular classes, so here's a copy function
        /// </summary>
        /// <returns> The copy </returns>
        virtual public TemplateTile Copy()
        {
            TemplateTile copy = new TemplateTile();
            copy.sprite = sprite;
            copy.tileType = tileType;
            copy.preferredTile = preferredTile;
            return copy;
        }
    }
}
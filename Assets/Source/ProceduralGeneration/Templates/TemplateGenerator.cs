using System.Collections.Generic;
using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// Generates a template, spawning all the tiles
    /// </summary>
    public class TemplateGenerator : MonoBehaviour
    {
        /// <summary>
        /// Generates the given template
        /// </summary>
        /// <param name="room"> The room that is having the template generated </param>
        /// <param name="template"> The template to generate </param>
        /// <param name="spawnEnemies"> Whether or not to spawn enemies </param>
        public void Generate(Room room, Template template, bool spawnEnemies = true)
        {
            room.template = Instantiate(template);
            room.template.transform.parent = room.transform;
            room.template.transform.localPosition = new Vector2(0, 0);
            room.livingEnemies = new List<GameObject>();

            GameObject tileContainer = new GameObject();
            tileContainer.name = "Tile Container";
            tileContainer.transform.parent = room.template.transform;
            tileContainer.transform.localPosition = new Vector3(-room.roomSize.x / 2, -room.roomSize.y / 2, 0);

            for (int i = 0; i < room.template.transform.childCount; i++)
            {
                Tile tile = room.template.transform.GetChild(i).GetComponent<Tile>();
                if (tile == null)
                {
                    throw new System.Exception(room.template.transform.GetChild(i) + " does not have a tile component!");
                }
                room.roomGrid[tile.gridLocation.x, tile.gridLocation.y] = tile;
            }
        }
    }
}
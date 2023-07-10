using System.Collections;
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
        /// <returns> Whether or not any enemies were spawned </returns>
        public bool Generate(Room room, Template template, bool spawnEnemies = true)
        {
            bool enemiesSpawned = false;

            room.template = Instantiate(template);
            room.template.name = template.name;
            room.template.transform.parent = room.transform;
            room.template.transform.localPosition = (Vector2) (-room.roomSize / 2);
            room.livingEnemies = new List<GameObject>();

            for (int i = 1; i < room.roomSize.x - 1; i++)
            {
                for (int j = 1; j < room.roomSize.y - 1; j++)
                {
                    Tile createdTile = null;
                    if (i < room.template.roomSize.x && j < room.template.roomSize.y && i >= 0 && j >= 0)
                    {
                        createdTile = room.template[i, j];
                    }

                    if (createdTile == null)
                    {
                        createdTile = new GameObject().AddComponent<Tile>();
                        createdTile.name = "Empty tile (" + i + ", " + j + ")";
                        createdTile.gridLocation = new Vector2Int(i, j);
                        createdTile.allowedMovementTypes = RoomInterface.MovementType.Walking | RoomInterface.MovementType.Burrowing | RoomInterface.MovementType.Flying;
                    }
                    else
                    {
                        createdTile.name = createdTile.name + " (" + i + ", " + j + ")";
                    }
                    createdTile.transform.parent = room.template.transform;
                    createdTile.transform.localPosition = new Vector3(i, j);
                    createdTile.room = room;
                    room.roomGrid[i, j] = createdTile;
                }
            }

            return spawnEnemies && enemiesSpawned;
        }
    }
}
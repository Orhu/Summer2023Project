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
            room.template.transform.localPosition = (Vector2) (-room.roomSize / 2);
            room.livingEnemies = new List<GameObject>();

            GameObject tileContainer = new GameObject();
            tileContainer.name = "Tile Container";
            tileContainer.transform.parent = room.template.transform;
            tileContainer.transform.localPosition = new Vector3(-room.roomSize.x / 2, -room.roomSize.y / 2);
            room.tileContainer = tileContainer;

            for (int i = 1; i < room.roomSize.x - 1; i++)
            {
                for (int j = 1; j < room.roomSize.y - 1; j++)
                {
                    if (i >= room.template.roomSize.x || j >= room.template.roomSize.y)
                    {
                        continue;
                    }

                    Tile createdTile = room.template[i, j];
                    if (createdTile == null)
                    {
                        createdTile = new GameObject().AddComponent<Tile>();
                        createdTile.gridLocation = new Vector2Int(i, j);
                        createdTile.allowedMovementTypes = RoomInterface.MovementType.Walk | RoomInterface.MovementType.Burrow | RoomInterface.MovementType.Fly;
                    }

                    createdTile.transform.localPosition = new Vector3(i, j);
                    createdTile.transform.parent = tileContainer.transform;
                    createdTile.room = room;
                    room.roomGrid[i, j] = createdTile;
                    createdTile.Enable();
                }
            }
        }
    }
}
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
            room.template = ScriptableObject.CreateInstance<Template>();
            room.template.roomSize = template.roomSize;
            room.template.tiles = template.tiles;
            room.livingEnemies = new List<GameObject>();

            if (template.enemyPools != null && template.enemyPools.enemyPools.Count != 0)
            {
                room.template.enemyPools = template.enemyPools.Copy();
                room.template.chosenEnemyPool = room.template.enemyPools.enemyPools[FloorGenerator.random.Next(0, room.template.enemyPools.enemyPools.Count)];
            }

            GameObject tileContainer = new GameObject();
            tileContainer.name = "Tile Container";
            tileContainer.transform.parent = room.transform;
            tileContainer.transform.localPosition = new Vector3(-room.roomSize.x / 2, -room.roomSize.y / 2, 0);

            for (int i = 1; i < room.roomSize.x - 1; i++)
            {
                for (int j = 1; j < room.roomSize.y - 1; j++)
                {
                    TemplateTile templateTile = null;

                    if (i < template.roomSize.x && j < template.roomSize.y)
                    {
                        templateTile = template.tiles[i][j];
                    }

                    Tile createdTile = ScriptableObject.CreateInstance<Tile>();

                    if (templateTile == null || templateTile.tileType == TileType.None)
                    {
                        createdTile.gridLocation = new Vector2Int(i, j);
                        createdTile.type = TileType.None;
                        createdTile.allowedMovementTypes = RoomInterface.MovementType.Walk | RoomInterface.MovementType.Burrow | RoomInterface.MovementType.Fly;

                    }
                    else
                    {
                        TemplateGenerationParameters templateGenParams = FloorGenerator.floorGeneratorInstance.templateGenerationParameters;
                        createdTile = templateGenParams.GetRandomTile(templateTile);
                        createdTile.gridLocation = new Vector2Int(i, j);
                        if (createdTile.spawnedObject != null && !(createdTile.type == TileType.EnemySpawner && !spawnEnemies))
                        {
                            createdTile.spawnedObject = Instantiate(createdTile.spawnedObject);
                            createdTile.spawnedObject.transform.parent = tileContainer.transform;
                            createdTile.spawnedObject.transform.localPosition = new Vector2(i, j);
                            createdTile.spawnedObject.SetActive(true);
                        }
                    }

                    room.roomGrid[i, j] = createdTile;
                }
            }
        }
    }
}
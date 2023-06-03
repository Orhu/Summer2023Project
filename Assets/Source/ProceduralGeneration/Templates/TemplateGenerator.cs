using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public void Generate(Room room, Template template)
    {
        room.template = ScriptableObject.CreateInstance<Template>();
        room.template.roomSize = template.roomSize;
        room.template.tiles = template.tiles;

        if (template.enemyPools != null)
        {
            room.template.enemyPools = ScriptableObject.CreateInstance<EnemyPools>();
            room.template.enemyPools.enemyPools = new List<EnemyPool>();
            foreach (EnemyPool enemyPool in template.enemyPools.enemyPools)
            {
                room.template.enemyPools.enemyPools.Add(enemyPool);
            }
            room.template.chosenEnemyPool = room.template.enemyPools.enemyPools[Random.Range(0, template.enemyPools.enemyPools.Count)];
        }
        GameObject tileContainer = new GameObject();
        tileContainer.name = "Tile Container";
        tileContainer.transform.parent = room.transform;
        tileContainer.transform.localPosition = new Vector3(-room.roomSize.x / 2, -room.roomSize.y / 2, 0);

        for (int i = 0; i < template.roomSize.x; i++)
        {
            for (int j = 0; j < template.roomSize.y; j++)
            { 
                TemplateTile templateTile = template.tiles[i][j];

                Tile createdTile = ScriptableObject.CreateInstance<Tile>();

                if (templateTile == null || templateTile.tileType == TileType.None)
                {
                    createdTile.gridLocation = new Vector2Int(i, j);
                    createdTile.type = TileType.None;
                    createdTile.walkable = true;

                }
                else
                {
                    TemplateGenerationParameters templateGenParams = FloorGenerator.floorGeneratorInstance.templateGenerationParameters;
                    createdTile = templateGenParams.GetRandomTile(templateTile);
                    createdTile.gridLocation = new Vector2Int(i, j);
                    if (createdTile.spawnedObject != null)
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

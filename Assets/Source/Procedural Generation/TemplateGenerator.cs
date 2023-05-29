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
        for (int i = 0; i < template.roomSize.x; i++)
        {
            for (int j = 0; j < template.roomSize.y; j++)
            { 
                TemplateTile templateTile = template.tiles[i, j];

                if (templateTile == null || templateTile.tileType == TileType.None)
                {
                    continue;
                }

                TemplateGenerationParameters templateGenParams = GetComponent<FloorGenerator>().floorGenerationParameters.templateGenerationParameters;
                Tile createdTile = templateGenParams.GetRandomTile(templateTile);
                createdTile.gridLocation = new Vector2Int(i, j);
                createdTile.spawnedObject = Instantiate(createdTile.spawnedObject);
                createdTile.spawnedObject.transform.parent = room.transform;
                createdTile.spawnedObject.transform.localPosition = new Vector2(i, j);
            }
        }
    }
}

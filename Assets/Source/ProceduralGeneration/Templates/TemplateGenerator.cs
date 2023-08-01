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

            List<GameObject> layers = room.template.GetLayers();
            if (layers.Count == 0)
            {
                throw new System.Exception("Template " + template.name + " has no layers!");
            }

            for (int i = 0; i < layers.Count; i++)
            {
                for (int j = 1; j < room.roomSize.x - 1; j++)
                {
                    for (int k = 1; k < room.roomSize.y - 1; k++)
                    {
                        GameObject tile = room.template[i, j, k];

                        ThingSpawner tileThingSpawner = null;
                        if (tile != null)
                        {
                            tileThingSpawner = tile.GetComponent<ThingSpawner>();

                        }

                        // Check if the tile at this location is an enemy
                        if (tile != null && (tile.CompareTag("Enemy") || (tileThingSpawner != null && tileThingSpawner.chosenThing != null && tileThingSpawner.chosenThing.CompareTag("Enemy"))))
                        {
                            enemiesSpawned = true;
                            tile.SetActive(spawnEnemies);
                        }

                        // Check if the spawned thing has a tile component (they aren't necessarily in the pathfinding layer) and make sure to set the room
                        if (tile != null && tile.GetComponent<Tile>() != null)
                        {
                            tile.GetComponent<Tile>().room = room;
                            tile.GetComponent<Tile>().gridLocation = new Vector2Int(j, k);
                        }

                        // If not pathfinding layer, then check for thing spawner then be done
                        if (i != 0) 
                        {
                            // Check if this tile is a thing spawner trying to spawn a tile not on the pathfinding layer
                            if (tileThingSpawner != null && tileThingSpawner.chosenThing != null && tileThingSpawner.chosenThing.GetComponent<Tile>() != null)
                            {
                                Debug.LogWarning("Thing spawner at " + tileThingSpawner.GetComponent<Tile>().gridLocation + " in layer " + layers[i].name +
                                                 "in template " + room.template.name + " is trying to spawn a tile not in the pathfinding layer! Disabling this thing spawner.");
                                tileThingSpawner.gameObject.SetActive(false);
                            }
                            continue; 
                        }

                        Tile createdTile = null;
                        if (j < room.template.roomSize.x && k < room.template.roomSize.y && j >= 0 && k >= 0)
                        {
                            createdTile = room.template[j, k];
                        }

                        if (createdTile == null)
                        {
                            createdTile = new GameObject().AddComponent<Tile>();
                            createdTile.name = "Empty tile (" + j + ", " + k + ")";
                            createdTile.gridLocation = new Vector2Int(j, k);
                            createdTile.transform.parent = layers[0].transform;
                            createdTile.transform.localPosition = new Vector3(j, k);
                        }
                        else
                        {
                            createdTile.name = createdTile.name + " (" + j + ", " + k + ")";
                        }
                        createdTile.room = room;
                        room.roomGrid[j, k] = createdTile;
                    }
                }
            }

            return spawnEnemies && enemiesSpawned;
        }
    }
}
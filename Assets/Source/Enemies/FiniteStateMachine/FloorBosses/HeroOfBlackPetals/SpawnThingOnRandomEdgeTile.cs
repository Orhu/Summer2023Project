using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// An action that spawns an thing at the aim location.
    /// </summary>
    [CreateAssetMenu(fileName = "NewSpawnThing", menuName = "FSM/Floor Boss/Hero of Black Petals/Spawn Thing At Random Edge")]
    public class SpawnThingOnRandomEdgeTile : Action
    {
        [Tooltip("The thing to spawn")]
        [SerializeField] private GameObject thing;

        [Tooltip("The delay before it is spawned")] [Min(0f)]
        [SerializeField] private float delay = 0f;

        /// <summary>
        /// Plays this action and causes all its effects.
        /// </summary>
        /// <param name="actor"> The actor that will be playing this action. </param>
        /// <param name="ignoredObjects"> The objects this action will ignore. </param>
        public override void Play(IActor actor, List<GameObject> ignoredObjects)
        {
            if (delay <= 0)
            {
                Vector2 randomTile = GetRandomTilePos();
                var instantiated = Instantiate(thing);
                Vector2 spawnPos = (Vector2)FloorGenerator.currentRoom.transform.position + randomTile;
                instantiated.transform.position = spawnPos;
            }
            else
            {
                actor.GetActionSourceTransform().GetComponent<MonoBehaviour>().StartCoroutine(DelayedSpawn(actor));
            }
        }

        private Vector2 GetRandomTilePos()
        {
            Room room = FloorGenerator.currentRoom;
            Tile[,] grid = room.roomGrid;

            float halfSizeX = room.roomSize.x / 2f;
            float halfSizeY = room.roomSize.y / 2f;

            Vector2[] possibleChoices = new[]
            { // addition and subtraction to avoid attempting spawns in walls
                new Vector2(halfSizeX - 1, halfSizeY - 1),
                new Vector2(-halfSizeX + 1, halfSizeY - 1),
                new Vector2(halfSizeX - 1, -halfSizeY + 1),
                new Vector2(-halfSizeX + 1, -halfSizeY + 1),
            };

            return possibleChoices[Random.Range(0, possibleChoices.Length)];
        }
        
        /// <summary>
        /// Spawns the thing.
        /// </summary>
        private IEnumerator DelayedSpawn(IActor actor)
        {
            yield return new WaitForSeconds(delay);
            Vector2 randomTile = GetRandomTilePos();
            Instantiate(thing).transform.position = (Vector2)FloorGenerator.currentRoom.transform.position + randomTile;
        }
    }
}
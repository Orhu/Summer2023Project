using System.Collections;
using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// Spawns a thing. Needs a tile component for easy reference to the grid location, but does not need to be put in the pathfinding layer, UNLESS
    /// it spawns things that DO need to be in the pathfinding layer. This is to avoid having 2 spawners on top of each other than both spawn a pathfinding
    /// tile. 
    /// </summary>
    [RequireComponent(typeof(Tile))]
    public class ThingSpawner : MonoBehaviour
    {
        [Tooltip("The list of things to choose from")]
        [SerializeField] private GameObjectLootTable things;

        // The thing that was chosen
        [HideInInspector] public GameObject chosenThing;

        /// <summary>
        /// Chooses the thing to spawn
        /// </summary>
        private void Awake()
        {
            if (!FloorGenerator.IsValid())
            {
                return;
            }

            if (things == null || things.weightedLoot.things == null || things.weightedLoot.things.Count <= 0)
            { 
                chosenThing = null;
                return;
            }

            // Currently this uses the floor generator random for everything; it should be fine since the thing is chosen before it's spawned, so
            // the generation should remain consistent.
            // However if we ever want thing spawners to use a different random I'll have to add support for that somehow.
            chosenThing = things.weightedLoot.GetRandomThing(FloorGenerator.random);
        }

        /// <summary>
        /// Spawns the thing
        /// </summary>
        private void Start()
        {
            if (chosenThing == null) 
            {
                Debug.LogWarning("Thing spawner in " + GetComponent<Tile>().room.template + " at " +  GetComponent<Tile>().gridLocation + " has no things specified! It will not spawn anything");
                return; 
            }

            string name = chosenThing.name;
            chosenThing = Instantiate(chosenThing.gameObject);
            Tile currentTile = GetComponent<Tile>();
            chosenThing.transform.parent = currentTile.room.template.transform;
            chosenThing.transform.localPosition = (Vector2) currentTile.gridLocation;
            chosenThing.name = name + " (" + currentTile.gridLocation.x + ", " + currentTile.gridLocation.y + ")";

            if (chosenThing.GetComponent<Tile>() != null)
            {
                Tile chosenThingTile = chosenThing.GetComponent<Tile>();
                chosenThingTile.gridLocation = currentTile.gridLocation;
                chosenThingTile.room = currentTile.room;

                // Only update the room grid if this actually affects pathfinding
                if (chosenThingTile.allowedMovementTypes != RoomInterface.MovementType.None)
                {
                    currentTile.room.roomGrid[currentTile.gridLocation.x, currentTile.gridLocation.y] = chosenThing.GetComponent<Tile>();
                }
            }
        }
    }
}
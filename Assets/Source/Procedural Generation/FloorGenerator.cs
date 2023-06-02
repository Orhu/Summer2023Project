using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles generation for an entire floor: The layout and the external rooms. 
/// The rooms handle their own internal generation, but they will use the parameters on this generator (the templates).
/// </summary>
public class FloorGenerator : MonoBehaviour
{
    [Tooltip("The generation parameters for this floor")]
    public FloorGenerationParameters floorGenerationParameters;

    // A reference to the generated map
    [HideInInspector] public Map map;

    // The room the player is currently in
    [HideInInspector] public Room currentRoom;

    // The instance
    public static FloorGenerator floorGeneratorInstance { get; private set; }

    /// <summary>
    /// Sets up the singleton
    /// </summary>
    void Awake()
    {
        if (floorGeneratorInstance != null && floorGeneratorInstance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        floorGeneratorInstance = this;
    }


    /// <summary>
    /// Generates the layout and exteriors of the room
    /// </summary>
    private void Start()
    {
        map = GetComponent<LayoutGenerator>().Generate(floorGenerationParameters.layoutGenerationParameters);
        GetComponent<RoomExteriorGenerator>().Generate(floorGenerationParameters.roomTypesToExteriorGenerationParameters, map, floorGenerationParameters.roomSize);
    }

    /// <summary>
    /// Transforms a map location to a world location
    /// </summary>
    /// <param name="mapLocation"> The location to transform </param>
    /// <param name="startLocation"> The start location (aka midpoint of the map) </param>
    /// <param name="roomSize"> The room size </param>
    /// <returns> The world location </returns>
    static public Vector2 TransformMapToWorld(Vector2Int mapLocation, Vector2Int startLocation, Vector2Int roomSize)
    {
        return new Vector2((mapLocation.x - startLocation.x) * roomSize.x, (mapLocation.y - startLocation.y) * roomSize.y);
    }
}

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

    /// <summary>
    /// Generates the layout and exteriors of the room
    /// </summary>
    private void Start()
    {
        map = GetComponent<LayoutGenerator>().Generate(floorGenerationParameters.layoutGenerationParameters);
        GetComponent<RoomExteriorGenerator>().Generate(floorGenerationParameters.roomTypesToExteriorGenerationParameters, map, floorGenerationParameters.roomSize);
    }
}

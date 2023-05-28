using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorGenerator : MonoBehaviour
{
    public FloorGenerationParameters floorGenerationParameters;

    [HideInInspector] public Map map;
    private void Start()
    {
        map = GetComponent<LayoutGenerator>().Generate(floorGenerationParameters.layoutGenerationParameters);
        GetComponent<LayoutGenerator>().SaveMap(map);
        GetComponent<RoomExteriorGenerator>().Generate(floorGenerationParameters.roomTypesToExteriorGenerationParameters, map, floorGenerationParameters.roomSize);
    }
}

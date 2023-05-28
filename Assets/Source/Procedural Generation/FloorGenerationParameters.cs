using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FloorGenerationParameters
{
    public RoomTypesToRoomExteriorGenerationParameters roomTypesToExteriorGenerationParameters;
    public LayoutGenerationParameters layoutGenerationParameters;
    public Vector2Int roomSize;
}

[System.Serializable]
public enum RoomType
{
    None,
    Normal,
    Start,
    Special,
    Boss,
    Exit
}

[System.Serializable]
public class RoomTypesToRoomExteriorGenerationParameters
{
    public List<RoomTypeToRoomExteriorGenerationParameters> roomTypesToRoomExteriorGenerationParameters;

    public RoomExteriorGenerationParameters At(RoomType roomType)
    {
        for (int i = 0; i < roomTypesToRoomExteriorGenerationParameters.Count; i++)
        {
            if (roomTypesToRoomExteriorGenerationParameters[i].roomType == roomType)
            {
                return roomTypesToRoomExteriorGenerationParameters[i].roomExteriorGenerationParameters;
            }
        }

        throw new System.Exception("No room of type " + roomType.ToString() + " in dictionary of room types to room exterior generation parameters");
    }
}

[System.Serializable]
public struct RoomTypeToRoomExteriorGenerationParameters
{
    public RoomType roomType;
    public RoomExteriorGenerationParameters roomExteriorGenerationParameters;
}

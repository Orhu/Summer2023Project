using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A scriptable object for storing data a specific effect a card can have on a dungeon.
/// </summary>
[CreateAssetMenu(fileName = "NewDungeonEffect", menuName = "Cards/DungeonEffect", order = 1)]
public class DungeonEffect : ScriptableObject
{
    //[SerializeField] RoomGenerationParameters changeInRoomGenerationParameters;

    [SerializeField] public string description = "";

    void Start()
    {
        
    }
    public string GetFormattedDescription()
    {
        return description;
    }

    public void Effect()
    {
        //ProceduralGeneration.proceduralGenerationInstance.AddRoomGenerationParameters(changeInRoomGenerationParameters);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDungeonEffect : ScriptableObject
{
    [SerializeField] RoomGenerationParameters changeInRoomGenerationParameters;

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
        ProceduralGeneration.proceduralGenerationInstance.AddRoomGenerationParameters(changeInRoomGenerationParameters);
    }
}

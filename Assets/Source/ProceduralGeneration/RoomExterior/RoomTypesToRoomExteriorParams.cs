using System.Collections.Generic;
using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// A dictionary that maps room types to exterior generation params
    /// </summary>
    [System.Serializable]
    [CreateAssetMenu(fileName = "NewRoomTypesToRoomExteriorParams", menuName = "Generation/RoomTypesToRoomExteriorParams")]
    public class RoomTypesToRoomExteriorParams : ScriptableObject
    {
        [Header("Tiles")]
        [Tooltip("The wall tile")]
        public Tile wallTile;

        [Tooltip("The door tile")]
        public Tile doorTile;

        [Tooltip("The floor prefab")]
        public GameObject floorTile;

        [Tooltip("The defuault exterior params to use if a given room type doesn't have specified room exterior params")]
        [SerializeField] public RoomExteriorParams defaultRoomExteriorParams;

        [Tooltip("A list of room types to exterior generation Params")]
        [SerializeField] public List<RoomTypeToRoomExteriorParams> roomTypesToRoomExteriorParams;

        /// <summary>
        /// Gets the exterior generation Params associated with the given room type
        /// </summary>
        /// <param name="roomType"> The room type to find the exterior generation Params of </param>
        /// <returns> The exterior generation Params </returns>
        public RoomExteriorParams At(RoomType roomType)
        {
            for (int i = 0; i < roomTypesToRoomExteriorParams.Count; i++)
            {
                if (roomTypesToRoomExteriorParams[i].roomType == roomType)
                {
                    return roomTypesToRoomExteriorParams[i].roomExteriorParams;
                }
            }

            throw new System.Exception("No room of type " + roomType.ToString() + " in dictionary of room types to room exterior generation Params");
        }

        /// <summary>
        /// Checks if the list of room types to exterior params contains the given room type
        /// </summary>
        /// <param name="roomType"> The room type to check </param>
        /// <returns> Whether or not that room type is in the list </returns>
        public bool Contains(RoomType roomType)
        {
            foreach (RoomTypeToRoomExteriorParams roomTypeToRoomExteriorParams in roomTypesToRoomExteriorParams)
            {
                if (roomTypeToRoomExteriorParams.roomType == roomType)
                {
                    return true;
                }
            }

            return false;
        }
    }

    /// <summary>
    /// A sturct that holds a room type and its associated exterior generation Params
    /// </summary>
    [System.Serializable]
    public struct RoomTypeToRoomExteriorParams
    {
        [Tooltip("The type")]
        [SerializeField] public RoomType roomType;

        [Tooltip("The generation Params associated with that type")]
        [SerializeField] public RoomExteriorParams roomExteriorParams;
    }
}
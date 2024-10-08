using System.Collections.Generic;
using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// A dictionary that maps room types to exterior generation params
    /// </summary>
    [System.Serializable]
    public class RoomTypesToRoomExteriorParams
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

        [Tooltip("A list of room types to exterior generation Params. The higher in this list a room type is, the higher priority its doors will have.")]
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

        /// <summary>
        /// Adds a room type and its exterior params
        /// </summary>
        /// <param name="roomTypeToRoomExteriorParams"> The room type and its exterior params </param>
        public void Add(RoomTypeToRoomExteriorParams roomTypeToRoomExteriorParams)
        {
            roomTypesToRoomExteriorParams.Add(roomTypeToRoomExteriorParams);
        }

        /// <summary>
        /// Adds a room type and its exterior params
        /// </summary>
        /// <param name="roomType"> The room type </param>
        /// <param name="exteriorParams"> The exterior params </param>
        public void Add(RoomType roomType, RoomExteriorParams exteriorParams)
        {
            roomTypesToRoomExteriorParams.Add(new RoomTypeToRoomExteriorParams(roomType, exteriorParams));
        }    

        /// <summary>
        /// Resets the room type to room exterior params list
        /// </summary>
        public void Reset()
        {
            foreach (RoomTypeToRoomExteriorParams roomTypeToRoomExteriorParams in roomTypesToRoomExteriorParams)
            {
                roomTypeToRoomExteriorParams.roomExteriorParams.Reset();
            }
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

        /// <summary>
        /// Constructor that takes a room type and room exterior params 
        /// </summary>
        /// <param name="roomType"> The room type </param>
        /// <param name="roomExteriorParams"> The room exterior params </param>
        public RoomTypeToRoomExteriorParams(RoomType roomType, RoomExteriorParams roomExteriorParams)
        {
            this.roomType = roomType;
            this.roomExteriorParams = roomExteriorParams;
        }
    }
}
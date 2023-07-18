using System.Collections.Generic;
using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// The Params that affect the layout generation
    /// </summary>
    [System.Serializable] [CreateAssetMenu(fileName = "NewLayoutParams", menuName = "Generation/LayoutParams")]
    public class LayoutParams : ScriptableObject
    {
        [Tooltip("A list of room types, and their layout Params")]
        public RoomTypesToLayoutParams roomTypesToLayoutParams;

        [Tooltip("The layout params that apply to the entire map")]
        public MapLayoutParams mapLayoutParams;
    }

    /// <summary>
    /// Stores the layout parameters that apply to the entire map
    /// </summary>
    [System.Serializable]
    public class MapLayoutParams
    {
        [Tooltip("The number of boss rooms to generate")] [Min(1)]
        public int numBossRooms = 1;

        [Tooltip("The number of doors that is preferred")] [Range(1, 4)]
        [SerializeField] public int preferredNumDoors = 2;

        [Tooltip("How strictly the generation adheres to the preferred number of doors (100 = very strict, 0 = not strict at all)")] [Range(0, 100)]
        [SerializeField] public float strictnessNumDoors = 12;
    }

    /// <summary>
    /// Stores the number of rooms and the variance of the number of rooms
    /// </summary>
    [System.Serializable]
    public class RoomLayoutParams
    {
        [Tooltip("The number of this type of room to appear in the layout")]
        public int numRooms;

        [Tooltip("The variance allowed with the number of rooms")]
        public int numRoomsVariance;
    }

    /// <summary>
    /// Stores a list of room types and their associated layout Params
    /// </summary>
    [System.Serializable]
    public class RoomTypesToLayoutParams
    {
        /// <summary>
        /// A list of room types to layout Params
        /// </summary>
        public List<RoomTypeToLayoutParams> roomTypesToLayoutParams;

        /// <summary>
        /// Gets the layout Params associated with a given room type
        /// </summary>
        /// <param name="roomType"> The room type </param>
        /// <returns> The layout Params </returns>
        public RoomTypeToLayoutParams At(RoomType roomType)
        {
            foreach (RoomTypeToLayoutParams roomTypeToLayoutParams in roomTypesToLayoutParams)
            {
                if (roomTypeToLayoutParams.roomType == roomType)
                {
                    return roomTypeToLayoutParams;
                }
            }

            throw new System.Exception("No room type of " + roomType.displayName + " in room types to layout Params");
        }

        /// <summary>
        /// Adds a room type to layout params to the list
        /// </summary>
        /// <param name="roomTypeToLayoutParams"> The params to add </param>
        public void Add(RoomTypeToLayoutParams roomTypeToLayoutParams)
        {
            roomTypesToLayoutParams.Add(roomTypeToLayoutParams);
        }

        /// <summary>
        /// Adds a room type to layout params to the list using the given room type and room layout params
        /// </summary>
        /// <param name="roomType"> The room type to add </param>
        /// <param name="roomLayoutParams"> The parameters of that room type </param>
        public void Add(RoomType roomType, RoomLayoutParams roomLayoutParams)
        {
            roomTypesToLayoutParams.Add(new RoomTypeToLayoutParams(roomType, roomLayoutParams));
        }
    }

    /// <summary>
    /// Stores a room type and its associated layout Params
    /// </summary>
    [System.Serializable]
    public class RoomTypeToLayoutParams
    {
        [Tooltip("The room type")]
        public RoomType roomType;

        [Tooltip("The layout params for this particular room type")]
        public RoomLayoutParams layoutParams;

        /// <summary>
        /// Default constructor
        /// </summary>
        public RoomTypeToLayoutParams() 
        {
            roomType = null;
            layoutParams = null;
        }

        /// <summary>
        /// Constructor taking a room type and layout params
        /// </summary>
        /// <param name="roomType"> The room type </param>
        /// <param name="layoutParams"> The layout params of that room type </param>
        public RoomTypeToLayoutParams(RoomType roomType, RoomLayoutParams layoutParams)
        {
            this.roomType = roomType;
            this.layoutParams = layoutParams;
        }
    }
}